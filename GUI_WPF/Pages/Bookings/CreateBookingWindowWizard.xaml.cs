using BUS;
using DTO;
using ET;
using GUI_WPF.Pages.Listings;
using GUI_WPF.UserControls.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI_WPF.Pages.Bookings
{
    /// <summary>
    /// Interaction logic for CreateBookingWindowWizard.xaml
    /// </summary>
    public partial class CreateBookingWindowWizard : Window
    {
        private int _currentStep = 1;
        private const int TOTAL_STEPS = 5;

        private readonly ET_Users _currentUser;

        private readonly BUS_Guest _guestBus = new BUS_Guest();
        private readonly BUS_User _userBus = new BUS_User();
        private List<DTO_GuestLookup> _allGuests = new List<DTO_GuestLookup>();
        private DTO_GuestLookup _selectedGuest;
        private readonly BUS_Items _itemBus = new BUS_Items();
        private readonly BUS_Area _areaBus = new BUS_Area();
        private List<DTO_ItemCard> _allListings = new List<DTO_ItemCard>();
        private DTO_ItemCard _selectedListing;
        private readonly BUS_ItemPrices _priceBus = new BUS_ItemPrices();
        private readonly BUS_CancellationPolicy _policyBus = new BUS_CancellationPolicy();
        private readonly BUS_Booking _bookingBus = new BUS_Booking();
        private List<DTO_CreateBookingNight> _nights = new List<DTO_CreateBookingNight>();
        private decimal _baseAmount;
        private decimal _discountAmount;
        private decimal _serviceFee;
        private decimal _taxAmount;
        private decimal _finalAmount;
        private bool _isStayValidated;
        private string _paymentStatus;
        private decimal _depositAmount;

        public CreateBookingWindowWizard(ET_Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            UpdateWizardState();
            LoadGuests();

            LoadAreas();

            LoadListings();
            LoadPolicies();
        }
        private void LoadPolicies()
        {
            cbPolicy.ItemsSource = _policyBus.GetAll();
            cbPolicy.DisplayMemberPath = "Name";
            cbPolicy.SelectedValuePath = "ID";

        }

        /// <summary>
        /// Hàm "đầu não" điều khiển hiển thị của 5 bước
        /// </summary>
        private void UpdateWizardState()
        {
            // 1. Ẩn toàn bộ Content
            GridStep1.Visibility = Visibility.Collapsed;
            GridStep2.Visibility = Visibility.Collapsed;
            GridStep3.Visibility = Visibility.Collapsed;
            GridStep4.Visibility = Visibility.Collapsed;
            GridStep5.Visibility = Visibility.Collapsed;

            // 2. Reset opacity của Stepper
            IndicatorStep1.Opacity = 0.5;
            IndicatorStep2.Opacity = 0.5;
            IndicatorStep3.Opacity = 0.5;
            IndicatorStep4.Opacity = 0.5;
            IndicatorStep5.Opacity = 0.5;

            // 3. Hiện Content & Đậm Indicator theo bước hiện tại
            switch (_currentStep)
            {
                case 1:
                    GridStep1.Visibility = Visibility.Visible;
                    IndicatorStep1.Opacity = 1.0;
                    btnBack.Visibility = Visibility.Collapsed;
                    btnNext.Content = "Tiếp theo";
                    break;
                case 2:
                    GridStep2.Visibility = Visibility.Visible;
                    IndicatorStep1.Opacity = 1.0; // Giữ sáng các bước đã qua (Tùy chọn)
                    IndicatorStep2.Opacity = 1.0;
                    btnBack.Visibility = Visibility.Visible;
                    btnNext.Content = "Tiếp theo";
                    break;
                case 3:
                    GridStep3.Visibility = Visibility.Visible;
                    IndicatorStep1.Opacity = 1.0;
                    IndicatorStep2.Opacity = 1.0;
                    IndicatorStep3.Opacity = 1.0;
                    btnBack.Visibility = Visibility.Visible;
                    btnNext.Content = "Kiểm tra phòng (Check)"; // Text đặc thù cho Step 3
                    break;
                case 4:
                    GridStep4.Visibility = Visibility.Visible;
                    IndicatorStep1.Opacity = 1.0;
                    IndicatorStep2.Opacity = 1.0;
                    IndicatorStep3.Opacity = 1.0;
                    IndicatorStep4.Opacity = 1.0;
                    LoadStep4();
                    btnBack.Visibility = Visibility.Visible;
                    btnNext.Content = "Tiếp theo";
                    break;
                case 5:
                    GridStep5.Visibility = Visibility.Visible;
                    IndicatorStep1.Opacity = 1.0;
                    IndicatorStep2.Opacity = 1.0;
                    IndicatorStep3.Opacity = 1.0;
                    IndicatorStep4.Opacity = 1.0;
                    IndicatorStep5.Opacity = 1.0;
                    btnBack.Visibility = Visibility.Visible;
                    LoadStep5();
                    btnNext.Content = "Xác nhận Booking"; // Nút chốt sổ
                    break;
            }
        }
        private void LoadGuests()
        {
            _allGuests = _guestBus.GetLookupData();
            dgGuests.ItemsSource = _allGuests;
        }
        private void LoadAreas()
        {
            cbArea.Items.Clear();

            cbArea.Items.Add("All");

            foreach (var area in _areaBus.GetAreasName())
            {
                cbArea.Items.Add(area.Name);
            }

            cbArea.SelectedIndex = 0;
        }
        private void LoadListings()
        {
            _allListings = _itemBus.GetItemCards();

            ApplyListingFilter();
        }
        private void OpenCalendar(DTO_ItemCard item)
        {
            ListingCalendarWindow window = new ListingCalendarWindow();

            window.LoadListing(item.ID, item.Title, item.ApproximateAddress);

            window.ShowDialog();
            LoadListings();
        }
        private void ApplyListingFilter()
        {
            IEnumerable<DTO_ItemCard> result = _allListings;

            string keyword = sbListing.SearchText?.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(x => (x.Title ?? "").ToLower().Contains(keyword) ||
                        (x.ApproximateAddress ?? "").ToLower().Contains(keyword));
            }

            if (cbArea.SelectedIndex > 0)
            {
                string area = cbArea.SelectedItem.ToString();

                result = result.Where(x => (x.AreaName ?? "").Equals(area, StringComparison.OrdinalIgnoreCase));
            }

            if (cbCapacity.SelectedIndex > 0)
            {
                ComboBoxItem item = cbCapacity.SelectedItem as ComboBoxItem;

                int minCapacity = int.Parse(item.Content.ToString().Replace("+", ""));

                result = result.Where(x => x.Capacity >= minCapacity);
            }

            RenderListings(result);
        }
        private void SelectListing(DTO_ItemCard item)
        {
            if (_selectedListing?.ID != item.ID)
            {
                ResetStayData();
            }

            _selectedListing = item;

            ListingEmptyState.Visibility = Visibility.Collapsed;
            SelectedListingPanel.Visibility = Visibility.Visible;
            txtListingTitle.Text = item.Title;
            txtListingArea.Text = item.ApproximateAddress;
            txtListingType.Text = item.Type;
            txtListingCapacity.Text = $"{item.Capacity} Guests";
            txtListingPrice.Text = item.MinPrice.HasValue ? $"${item.MinPrice:0}" : "No Price";
            txtListingStatus.Text = item.Status;
        }
        private void ResetStayData()
        {
            _isStayValidated = false;

            _nights.Clear();

            _baseAmount = 0;
            _discountAmount = 0;
            _serviceFee = 0;
            _taxAmount = 0;
            _finalAmount = 0;

            txtSummaryStatus.Text = "Waiting for validation...";

            txtSummaryNights.Text = "";

            txtSummaryPrice.Text = "";
        }
        private void RenderListings(IEnumerable<DTO_ItemCard> listings)
        {
            wpListings.Children.Clear();

            foreach (var item in listings)
            {
                ListingCard card =
                    new ListingCard();

                card.Cursor =
                    Cursors.Hand;

                card.LoadListing(item);

                card.PricingClicked += (s, e) => { OpenCalendar(item); };
                card.AvailabilityClicked += (s, e) => { OpenCalendar(item); };
                card.OpenClicked += (s, e) => { SelectListing(item); };
                card.MouseLeftButtonUp += (s, e) => { SelectListing(item); };

                wpListings.Children.Add(card);
            }
        }

        private void LoadStep4()
        {
            wpNightCards.Children.Clear();

            foreach (var night in _nights)
            {
                BookingNightCard card = new BookingNightCard();

                card.Width = 260;

                card.Margin = new Thickness(0, 0, 12, 12);

                card.LoadNight(night);

                wpNightCards.Children.Add(card);
            }

            txtBaseAmount.Text = $"{_baseAmount:N0} ₫";
            txtDiscountAmount.Text = $"{_discountAmount:N0} ₫";
            txtServiceFee.Text = $"{_serviceFee:N0} ₫";
            txtTaxAmount.Text = $"{_taxAmount:N0} ₫";
            txtFinalAmount.Text = $"{_finalAmount:N0} ₫";
        }
        private void LoadStep5()
        {
            txtReviewGuestName.Text = _selectedGuest.FullName;
            txtReviewGuestEmail.Text = _selectedGuest.Email;
            txtReviewGuestPhone.Text = _selectedGuest.PhoneNumber;
            txtReviewListingTitle.Text = _selectedListing.Title;
            txtReviewListingArea.Text = _selectedListing.ApproximateAddress;
            txtReviewDates.Text = $"{dpCheckIn.SelectedDate:dd/MM/yyyy} - {dpCheckOut.SelectedDate:dd/MM/yyyy}";
            txtReviewGuests.Text = $"{txtGuests.Text} Guest(s)";
            txtReviewPolicy.Text = cbPolicy.Text;
            txtReviewFinalAmount.Text = $"{_finalAmount:N0} ₫";
        }
        private void sbListing_TextChanged(object sender, TextChangedEventArgs e) => ApplyListingFilter();
        private void cbArea_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyListingFilter();
        private void cbCapacity_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyListingFilter();
        private void sbGuest_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = sbGuest.SearchText?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                dgGuests.ItemsSource = _allGuests;

                return;
            }

            dgGuests.ItemsSource = _allGuests.Where(x =>
                                    (x.FullName ?? "").ToLower().Contains(keyword) ||
                                    (x.Email ?? "").ToLower().Contains(keyword) ||
                                    (x.PhoneNumber ?? "").ToLower().Contains(keyword)).ToList();
        }
        private void dgGuests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgGuests.SelectedItem == null)
                return;

            _selectedGuest = (DTO_GuestLookup)dgGuests.SelectedItem;
            GuestEmptyState.Visibility = Visibility.Collapsed;
            SelectedGuestPanel.Visibility = Visibility.Visible;
            txtGuestName.Text = _selectedGuest.FullName;
            txtGuestEmail.Text = _selectedGuest.Email;
            txtGuestPhone.Text = _selectedGuest.PhoneNumber;
            txtGuestCountry.Text = _selectedGuest.Country;
        }
        private void GenerateNightPricing(DateTime checkIn, DateTime checkOut)
        {
            _nights.Clear();

            var prices = _priceBus.GetPricesInRange(_selectedListing.ID, checkIn, checkOut);

            _baseAmount = 0;

            foreach (var p in prices)
            {
                _nights.Add(new DTO_CreateBookingNight
                {
                    Date = p.Date,
                    ItemPriceID = p.ID,
                    Price = p.Price,
                    CancellationPolicyID = p.CancellationPolicyID,
                    PolicyName = cbPolicy.Text
                });

                _baseAmount += p.Price;
            }

            _discountAmount = 0;
            _serviceFee = 0;
            _taxAmount = 0;
            _finalAmount = _baseAmount;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStep == 1)
            {
                if (!ValidateStep1())
                    return;
            }
            if (_currentStep == 2)
            {
                if (!ValidateStep2())
                    return;
            }
            if (_currentStep == 3)
            {
                if (!_isStayValidated)
                {
                    MessageBox.Show("Please validate stay information first.");

                    return;
                }
            }
            if (_currentStep == 5)
            {
                if (!ValidateStep5())
                    return;
            }

            if (_currentStep < TOTAL_STEPS)
            {
                _currentStep++;
                UpdateWizardState();
            }
            else
            {
                DTO_CreateBooking dto = BuildBookingDto();

                string error;

                bool success = _bookingBus.CreateManualBooking(dto, out error);

                if (!success)
                {
                    MessageBox.Show(error, "Create Booking", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }

                MessageBox.Show("Booking created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;

                Close();
            }
        }
        private DTO_CreateBooking BuildBookingDto()
        {
            return new DTO_CreateBooking
            {
                GuestUserID = _selectedGuest.UserID,
                GuestFullName = _selectedGuest.FullName,
                GuestEmail = _selectedGuest.Email,
                GuestPhone = _selectedGuest.PhoneNumber,
                ItemID = _selectedListing.ID,
                ListingTitle = _selectedListing.Title,
                CheckInDate = dpCheckIn.SelectedDate.Value,
                CheckOutDate = dpCheckOut.SelectedDate.Value,
                NumberOfGuests = int.Parse(txtGuests.Text),
                CancellationPolicyID = (long)cbPolicy.SelectedValue,
                SpecialRequests = txtRequests.Text,
                Nights = _nights,
                BaseAmount = _baseAmount,
                DiscountAmount = _discountAmount,
                ServiceFee = _serviceFee,
                TaxAmount = _taxAmount,
                FinalAmount = _finalAmount,
                CreatedByUserID = _currentUser.ID,
                PaymentStatus = _paymentStatus,
                DepositAmount = _depositAmount
            };
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStep > 1)
            {
                _currentStep--;
                UpdateWizardState();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn hủy tiến trình đặt phòng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Close();
            }
        }
        private void btnCheckStay_Click(object sender, RoutedEventArgs e)
        {
            _isStayValidated = false;

            if (_selectedListing == null)
            {
                MessageBox.Show("Please select a listing.");

                return;
            }

            if (!dpCheckIn.SelectedDate.HasValue || !dpCheckOut.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select dates.");

                return;
            }

            DateTime checkIn = dpCheckIn.SelectedDate.Value.Date;

            DateTime checkOut = dpCheckOut.SelectedDate.Value.Date;

            if (!int.TryParse(txtGuests.Text, out int guests))
            {
                MessageBox.Show("Invalid guest count.");

                return;
            }
            if (guests > _selectedListing.Capacity)
            {
                MessageBox.Show(
                    $"Maximum capacity is {_selectedListing.Capacity} guests.");

                return;
            }

            if (cbPolicy.SelectedValue == null)
            {
                MessageBox.Show("Please select a cancellation policy.");

                return;
            }

            if (!_bookingBus.ValidateAvailability(_selectedListing.ID, checkIn, checkOut))
            {
                MessageBox.Show("Listing unavailable.");

                return;
            }

            if (!_bookingBus.ValidateBookingConflict(_selectedListing.ID, checkIn, checkOut))
            {
                MessageBox.Show("Listing already booked.");

                return;
            }

            if (!_bookingBus.ValidateDateOverlap(_selectedListing.ID, checkIn, checkOut))
            {
                MessageBox.Show("Booking overlap detected.");

                return;
            }


            GenerateNightPricing(checkIn, checkOut);

            if (_nights.Count == 0)
            {
                MessageBox.Show("No pricing found for selected dates.");

                return;
            }

            _isStayValidated = true;

            txtSummaryStatus.Text = "Available ✓";

            txtSummaryNights.Text = $"{_nights.Count} night(s)";
            txtSummaryPrice.Text = $"{_finalAmount:N0} ₫";
        }
        private bool ValidateStep1()
        {
            if (_selectedGuest == null)
            {
                MessageBox.Show("Please select a guest.");
                return false;
            }

            return true;
        }
        private bool ValidateStep2()
        {
            if (_selectedListing == null)
            {
                MessageBox.Show(
                    "Please select a listing.");

                return false;
            }

            return true;
        }
        private bool ValidateStep5()
        {
            if (cbPaymentStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select payment status.");

                return false;
            }

            return true;
        }

        private void btnNewGuest_Click(object sender, RoutedEventArgs e)
        {
            CreateGuestDialog dlg = new CreateGuestDialog();

            dlg.Owner = Window.GetWindow(this);

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                LoadGuests();
            }
        }
    }
}
