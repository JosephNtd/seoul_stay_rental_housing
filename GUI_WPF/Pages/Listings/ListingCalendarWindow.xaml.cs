using BUS;
using ET;
using GUI_WPF.UserControls.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.Pages.Listings
{
    public partial class ListingCalendarWindow : Window
    {
        public ListingCalendarWindow()
        {
            InitializeComponent();
            _currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _selectedDates = new HashSet<DateTime>();
            RenderCalendar();
        }

        // BUS
        private readonly BUS_ItemPrices _busPrices = new BUS_ItemPrices();
        private readonly BUS_ItemAvailability _busAvailability = new BUS_ItemAvailability();

        // DATA
        private DateTime _currentMonth;
        private readonly HashSet<DateTime> _selectedDates = new HashSet<DateTime>();
        private long _itemId;
        private string _listingTitle;
        private string _listingAddress;

        private readonly List<ET_ItemAvailability> _availabilities = new List<ET_ItemAvailability>();
        private readonly List<ET_ItemPrices> _prices = new List<ET_ItemPrices>();

        // LOAD
        public void LoadListing(long itemId, string title, string address)
        {
            _itemId = itemId;
            _listingTitle = title;
            _listingAddress = address;
            txtListingTitle.Text = title;
            txtListingAddress.Text = address;

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            _availabilities.Clear();
            _prices.Clear();

            // 1. Load Prices
            var pricesData = _busPrices.GetPrices(_itemId);
            if (pricesData != null)
                _prices.AddRange(pricesData);

            // 2. Load Availabilities bằng BUS_ItemAvailability mới
            var availabilityData = _busAvailability.GetAvailabilities(_itemId);
            if (availabilityData != null)
                _availabilities.AddRange(availabilityData);

            // 3. Vẽ lại lịch
            RenderCalendar();
        }

        // RENDER
        private void RenderCalendar()
        {
            CalendarGrid.Children.Clear();
            txtCurrentMonth.Text = _currentMonth.ToString("MMMM yyyy");

            DateTime firstDay = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            int offset = (int)firstDay.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);

            for (int i = 0; i < offset; i++)
                CalendarGrid.Children.Add(new Border());

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);

                var price = _prices.FirstOrDefault(x => x.Date.Date == currentDate.Date);

                // SỬA TẠI ĐÂY: Tìm trạng thái khả dụng thực tế từ Database
                var availability = _availabilities.FirstOrDefault(x => x.Date.Date == currentDate.Date);
                // Nếu database chưa có bản ghi nào cho ngày này, mặc định hiểu là True (Available)
                bool isAvailable = availability == null ? true : availability.IsAvailable;

                decimal? nightlyPrice = price?.Price;

                CalendarDayCell cell = new CalendarDayCell();
                cell.Margin = new Thickness(8);
                cell.LoadDate(currentDate, isAvailable, nightlyPrice);
                cell.IsSelected = _selectedDates.Contains(currentDate);

                cell.DayClicked += Cell_DayClicked;
                CalendarGrid.Children.Add(cell);
            }

            UpdateStats();
        }

        private void UpdateStats()
        {
            // Đếm số ngày bị Blocked (IsAvailable = false) thuộc tháng và năm đang xem trên lịch
            int blockedDays = _availabilities.Count(x => x.Date.Year == _currentMonth.Year
                                                     && x.Date.Month == _currentMonth.Month
                                                     && x.IsAvailable == false);

            int totalDays = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
            int availableDays = totalDays - blockedDays;

            // Cập nhật lên giao diện
            txtAvailableDays.Text = $"{availableDays} Days Available";
            txtBlockedDays.Text = $"{blockedDays} Blocked";
        }

        private void Cell_DayClicked(object sender, EventArgs e)
        {
            if (sender is CalendarDayCell cell)
            {
                DateTime date = cell.Date;
                if (_selectedDates.Contains(date))
                    _selectedDates.Remove(date);
                else
                    _selectedDates.Add(date);

                cell.IsSelected = _selectedDates.Contains(date);
                UpdateSelectedInfo();
            }
        }

        private void UpdateSelectedInfo()
        {
            if (_selectedDates.Count == 0)
            {
                txtSelectedDate.Text = "No date selected";
                txtSelectedStatus.Text = "Availability: —";
                txtSelectedPrice.Text = "Price: —";
            }
            else if (_selectedDates.Count == 1)
            {
                var date = _selectedDates.First();
                txtSelectedDate.Text = date.ToString("dddd, MMMM dd yyyy");

                // Hiển thị giá
                var price = _prices.FirstOrDefault(x => x.Date.Date == date);
                txtSelectedPrice.Text = price != null ? $"Price: ${price.Price:0}" : "Price: Default";

                // SỬA TẠI ĐÂY: Hiển thị trạng thái khả dụng khi click vào 1 ngày
                var availability = _availabilities.FirstOrDefault(x => x.Date.Date == date);
                bool isAvail = availability == null ? true : availability.IsAvailable;
                txtSelectedStatus.Text = isAvail ? "Availability: Available" : "Availability: Blocked";
            }
            else
            {
                txtSelectedDate.Text = $"{_selectedDates.Count} dates selected";
                txtSelectedPrice.Text = "Multiple dates";
                txtSelectedStatus.Text = "Multiple status";
            }
        }

        // MONTH NAV
        private void btnPrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            RenderCalendar();
        }

        private void btnNextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            RenderCalendar();
        }

        // EDIT PRICE 
        private void btnEditPrice_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDates.Count == 0)
            {
                MessageBox.Show("Please select at least one date.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var firstDate = _selectedDates.First();
            var existingPrice = _prices.FirstOrDefault(x => x.Date.Date == firstDate.Date);

            if (existingPrice == null)
            {
                existingPrice = new ET_ItemPrices
                {
                    ItemID = _itemId,
                    Date = firstDate,
                    Price = 100
                };
            }

            PriceEditorPopup priceEditor = new PriceEditorPopup();
            priceEditor.LoadData(existingPrice, _busPrices.GetPolicies(), new List<ET_CancellationRefundFees>());

            Window popupWindow = new Window
            {
                Title = "Edit Price",
                Content = priceEditor,
                Width = 560,
                Height = 760,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = System.Windows.Media.Brushes.Transparent,
                WindowStyle = WindowStyle.None
            };

            priceEditor.SaveClicked += (s, args) =>
            {
                // Lưu vào Database
                bool success = _busPrices.SetPrice(
                    _itemId,
                    existingPrice.Date,
                    existingPrice.Price,
                    existingPrice.CancellationPolicyID == 0 ? 1L : existingPrice.CancellationPolicyID); // default policy

                if (success)
                {
                    LoadDataFromDatabase();
                    MessageBox.Show("Price updated successfully!");
                    MessageBox.Show("Price updated successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to update price.");
                }

                popupWindow.Close();
            };

            priceEditor.CancelClicked += (s, args) => popupWindow.Close();
            priceEditor.CloseClicked += (s, args) => popupWindow.Close();

            popupWindow.ShowDialog();
        }

        // BULK ACTIONS
        private void btnBulkAvailable_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDates.Count == 0) return;

            // Gọi BUS xử lý Batch Update bằng LINQ gọn gàng trong 1 dòng
            bool success = _busAvailability.SetAvailabilityForDates(_itemId, _selectedDates.ToList(), true);

            if (success)
            {
                LoadDataFromDatabase();
            }
            else
            {
                MessageBox.Show("Cập nhật trạng thái khả dụng thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBulkBlocked_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDates.Count == 0) return;

            // Gọi BUS xử lý Batch Update bằng LINQ gọn gàng trong 1 dòng
            bool success = _busAvailability.SetAvailabilityForDates(_itemId, _selectedDates.ToList(), false);

            if (success)
            {
                LoadDataFromDatabase();
            }
            else
            {
                MessageBox.Show("Khóa các ngày đã chọn thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}