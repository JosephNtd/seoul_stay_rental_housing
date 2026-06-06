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
using Helper;              // Helper_QRCode, Helper_InvoicePDF
using GUI_WPF.Helpers;     // SignalRService

namespace GUI_WPF.Pages.Bookings
{
    /// <summary>
    /// Interaction logic for CreateBookingWindowWizard.xaml
    /// </summary>
    public partial class CreateBookingWindowWizard : Window
    {
        private int _currentStep = 1;
        private const int TOTAL_STEPS = 6;

        private readonly ET_Users _currentUser;

        // STEP 1: GUEST
        private readonly BUS_Guest _guestBus = new BUS_Guest();
        private readonly BUS_User _userBus = new BUS_User();
        private List<DTO_GuestLookup> _allGuests = new List<DTO_GuestLookup>();
        private DTO_GuestLookup _selectedGuest;

        // STEP 2: LISTING
        private readonly BUS_Items _itemBus = new BUS_Items();
        private readonly BUS_Area _areaBus = new BUS_Area();
        private List<DTO_ItemCard> _allListings = new List<DTO_ItemCard>();
        private DTO_ItemCard _selectedListing;

        // STEP 3: STAY
        private readonly BUS_ItemPrices _priceBus = new BUS_ItemPrices();
        private readonly BUS_CancellationPolicy _policyBus = new BUS_CancellationPolicy();
        private List<DTO_CreateBookingNight> _nights = new List<DTO_CreateBookingNight>();
        private bool _isStayValidated;

        // STEP 4: ADDON SERVICES (MỚI)
        private readonly BUS_Services _serviceBus = new BUS_Services();
        private readonly BUS_ServiceType _serviceTypeBus = new BUS_ServiceType();
        private List<ET_ServiceType> _serviceTypes = new List<ET_ServiceType>();
        private List<ET_Services> _allServices = new List<ET_Services>();
        private List<DTO_CreateBookingAddon> _addonList = new List<DTO_CreateBookingAddon>();
        private decimal _addonTotal;

        // STEP 5: PRICING
        private readonly BUS_Booking _bookingBus = new BUS_Booking();
        private readonly BUS_Coupon _couponBus = new BUS_Coupon();
        private decimal _baseAmount;
        private decimal _discountAmount;
        private decimal _serviceFee;
        private decimal _taxAmount;
        private decimal _finalAmount;
        private ET_Coupons _appliedCoupon;

        // STEP 6: PAYMENT
        private readonly BUS_TransactionType _transTypeBus = new BUS_TransactionType();
        private readonly BUS_Invoice _invoiceBus = new BUS_Invoice();

        // CONSTRUCTOR
        public CreateBookingWindowWizard(ET_Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            UpdateWizardState();
            LoadGuests();

            LoadAreas();

            LoadListings();
            LoadPolicies();
            LoadTransactionTypes();
        }


        // INIT LOADERS


        private void LoadPolicies()
        {
            cbPolicy.ItemsSource = _policyBus.GetAll();
            cbPolicy.SelectedValuePath = "ID";
        }
        private void LoadTransactionTypes()
        {
            var types = _transTypeBus.GetData();
            cbTransactionType.ItemsSource = types;
            cbTransactionType.SelectedValuePath = "ID";
            if (types.Count > 0)
                cbTransactionType.SelectedIndex = 0;
        }


        // WIZARD STATE CONTROLLER (6 STEPS)


        /// <summary>
        /// Hàm "đầu não" điều khiển hiển thị của 6 bước
        /// </summary>
        private void UpdateWizardState()
        {
            // 1. Ẩn toàn bộ Content
            GridStep1.Visibility = Visibility.Collapsed;
            GridStep2.Visibility = Visibility.Collapsed;
            GridStep3.Visibility = Visibility.Collapsed;
            GridStep4.Visibility = Visibility.Collapsed;
            GridStep5.Visibility = Visibility.Collapsed;
            GridStep6.Visibility = Visibility.Collapsed;

            // 2. Reset opacity của Stepper
            IndicatorStep1.Opacity = 0.5;
            IndicatorStep2.Opacity = 0.5;
            IndicatorStep3.Opacity = 0.5;
            IndicatorStep4.Opacity = 0.5;
            IndicatorStep5.Opacity = 0.5;
            IndicatorStep6.Opacity = 0.5;

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
                    IndicatorStep1.Opacity = 1.0;
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
                    btnNext.Content = "Kiểm tra phòng (Check)";
                    break;
                case 4:
                    GridStep4.Visibility = Visibility.Visible;
                    IndicatorStep1.Opacity = 1.0;
                    IndicatorStep2.Opacity = 1.0;
                    IndicatorStep3.Opacity = 1.0;
                    IndicatorStep4.Opacity = 1.0;
                    LoadStep4_Addon();
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
                    LoadStep5();
                    btnBack.Visibility = Visibility.Visible;
                    btnNext.Content = "Tiếp theo";
                    break;
                case 6:
                    GridStep6.Visibility = Visibility.Visible;
                    IndicatorStep1.Opacity = 1.0;
                    IndicatorStep2.Opacity = 1.0;
                    IndicatorStep3.Opacity = 1.0;
                    IndicatorStep4.Opacity = 1.0;
                    IndicatorStep5.Opacity = 1.0;
                    IndicatorStep6.Opacity = 1.0;
                    btnBack.Visibility = Visibility.Visible;
                    LoadStep6();
                    btnNext.Content = "Xác nhận Booking";
                    break;
            }
        }


        // STEP 1: GUEST


        private void LoadGuests()
        {
            _allGuests = _guestBus.GetLookupData();
            dgGuests.ItemsSource = _allGuests;
        }
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
        private bool ValidateStep1()
        {
            if (_selectedGuest == null)
            {
                MessageBox.Show("Please select a guest.");
                return false;
            }

            return true;
        }


        // STEP 2: LISTING


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
        private void sbListing_TextChanged(object sender, TextChangedEventArgs e) => ApplyListingFilter();
        private void cbArea_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyListingFilter();
        private void cbCapacity_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyListingFilter();

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


        // STEP 3: STAY


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
                    PolicyName = (cbPolicy.SelectedItem as ET_CancellationPolicies)?.Name ?? ""
                });

                _baseAmount += p.Price;
            }

            _serviceFee = 0;
            _taxAmount = 0;

            RecalculatePricing();
        }


        // STEP 4: ADDON SERVICES (MỚI HOÀN TOÀN)


        private void LoadStep4_Addon()
        {
            // Load service types vào ComboBox (chỉ load 1 lần)
            if (_serviceTypes.Count == 0)
            {
                _serviceTypes = _serviceTypeBus.GetData();

                cbServiceType.Items.Clear();

                // Thêm "All" option
                cbServiceType.Items.Add("All");

                foreach (var type in _serviceTypes)
                {
                    cbServiceType.Items.Add(type.Name);
                }

                cbServiceType.SelectedIndex = 0;
            }

            // Load tất cả services
            _allServices = _serviceBus.GetAll();

            // Render service cards
            RenderServiceCards(_allServices);

            // Refresh addon list bên phải
            RenderSelectedAddons();
        }
        private void cbServiceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbServiceType.SelectedIndex < 0)
                return;

            if (cbServiceType.SelectedIndex == 0)
            {
                // "All" — hiển thị tất cả
                RenderServiceCards(_allServices);
            }
            else
            {
                string typeName = cbServiceType.SelectedItem.ToString();

                var filtered = _allServices
                    .Where(x => (x.ServiceTypeName ?? "").Equals(typeName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                RenderServiceCards(filtered);
            }
        }
        private void RenderServiceCards(List<ET_Services> services)
        {
            spServiceCards.Children.Clear();

            foreach (var service in services)
            {
                // Card Container
                Border card = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66FFFFFF")),
                    CornerRadius = new CornerRadius(16),
                    Padding = new Thickness(18),
                    Margin = new Thickness(0, 0, 0, 12)
                };

                StackPanel cardContent = new StackPanel();

                // ROW 1: Service Name + Type + Price
                Grid headerRow = new Grid();
                headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                StackPanel namePanel = new StackPanel();
                TextBlock txtName = new TextBlock
                {
                    Text = service.Name,
                    FontWeight = FontWeights.Bold,
                    FontSize = 15,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3131"))
                };
                TextBlock txtType = new TextBlock
                {
                    Text = service.ServiceTypeName,
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B7D7D")),
                    Margin = new Thickness(0, 2, 0, 0)
                };
                namePanel.Children.Add(txtName);
                namePanel.Children.Add(txtType);
                Grid.SetColumn(namePanel, 0);
                headerRow.Children.Add(namePanel);

                TextBlock txtPrice = new TextBlock
                {
                    Text = $"{service.Price:N0} ₫/pax",
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 14,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2AA876")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(txtPrice, 1);
                headerRow.Children.Add(txtPrice);

                cardContent.Children.Add(headerRow);

                // Capacity Info
                TextBlock txtCap = new TextBlock
                {
                    Text = $"Daily Cap: {service.DailyCap}  ·  Booking Cap: {service.BookingCap}",
                    FontSize = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B7D7D")),
                    Margin = new Thickness(0, 6, 0, 10)
                };
                cardContent.Children.Add(txtCap);

                // ROW 2: Input Fields
                Grid inputRow = new Grid();
                inputRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                inputRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
                inputRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
                inputRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
                inputRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });

                // Number of People
                StackPanel paxPanel = new StackPanel();
                paxPanel.Children.Add(new TextBlock
                {
                    Text = "Pax",
                    FontSize = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                    Margin = new Thickness(0, 0, 0, 4)
                });
                TextBox txtPax = new TextBox
                {
                    Text = "1",
                    Style = (Style)FindResource("SoftTextBoxStyle"),
                    Height = 34,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(8, 0, 8, 0)
                };
                paxPanel.Children.Add(txtPax);
                Grid.SetColumn(paxPanel, 0);
                inputRow.Children.Add(paxPanel);

                // Date
                StackPanel datePanel = new StackPanel();
                datePanel.Children.Add(new TextBlock
                {
                    Text = "Date",
                    FontSize = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                    Margin = new Thickness(0, 0, 0, 4)
                });
                DatePicker dpFrom = new DatePicker
                {
                    Height = 34,
                    Style = (Style)FindResource("SoftDatePickerStyle"),
                    SelectedDate = dpCheckIn.SelectedDate
                };
                datePanel.Children.Add(dpFrom);
                Grid.SetColumn(datePanel, 2);
                inputRow.Children.Add(datePanel);

                // Notes
                StackPanel notesPanel = new StackPanel();
                notesPanel.Children.Add(new TextBlock
                {
                    Text = "Notes",
                    FontSize = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                    Margin = new Thickness(0, 0, 0, 4)
                });
                TextBox txtNotes = new TextBox
                {
                    Height = 34,
                    Style = (Style)FindResource("SoftTextBoxStyle"),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(8, 0, 8, 0)
                };
                notesPanel.Children.Add(txtNotes);
                Grid.SetColumn(notesPanel, 4);
                inputRow.Children.Add(notesPanel);

                cardContent.Children.Add(inputRow);

                // ROW 3: Add Button
                Button btnAdd = new Button
                {
                    Content = "+ Add to Booking",
                    Height = 34,
                    Margin = new Thickness(0, 10, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Padding = new Thickness(16, 0, 16, 0),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7B5455")),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 12,
                    Cursor = Cursors.Hand,
                    Style = (Style)FindResource("PrimaryButtonStyle"),
                };

                // Capture references cho closure
                ET_Services currentService = service;
                TextBox capturedPax = txtPax;
                DatePicker capturedDate = dpFrom;
                TextBox capturedNotes = txtNotes;

                btnAdd.Click += (s, ev) =>
                {
                    AddAddonToList(currentService, capturedPax, capturedDate, capturedNotes);
                };

                cardContent.Children.Add(btnAdd);

                card.Child = cardContent;
                spServiceCards.Children.Add(card);
            }
        }
        private void AddAddonToList(ET_Services service, TextBox txtPax, DatePicker dpFrom, TextBox txtNotes)
        {
            // Validate Pax
            if (!long.TryParse(txtPax.Text, out long pax) || pax < 1)
            {
                MessageBox.Show("Please enter a valid number of people (≥ 1).");
                return;
            }

            // Validate Date
            if (!dpFrom.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date.");
                return;
            }

            DateTime fromDate = dpFrom.SelectedDate.Value.Date;

            // Validate: Date phải nằm trong stay period
            if (dpCheckIn.SelectedDate.HasValue && dpCheckOut.SelectedDate.HasValue)
            {
                if (fromDate < dpCheckIn.SelectedDate.Value.Date ||
                    fromDate >= dpCheckOut.SelectedDate.Value.Date)
                {
                    MessageBox.Show(
                        $"Date must be within the stay period " +
                        $"({dpCheckIn.SelectedDate.Value:dd/MM/yyyy} – {dpCheckOut.SelectedDate.Value:dd/MM/yyyy}).");
                    return;
                }
            }

            // Tạo DTO
            var addon = new DTO_CreateBookingAddon
            {
                ServiceID = service.ID,
                ServiceName = service.Name,
                ServiceTypeName = service.ServiceTypeName,
                Price = service.Price,
                NumberOfPeople = pax,
                FromDate = fromDate,
                Notes = txtNotes.Text?.Trim() ?? ""
            };

            _addonList.Add(addon);

            RecalculateAddonTotal();

            RenderSelectedAddons();

            // Reset input
            txtPax.Text = "1";
            txtNotes.Text = "";
        }
        private void RemoveAddon(DTO_CreateBookingAddon addon)
        {
            _addonList.Remove(addon);

            RecalculateAddonTotal();

            RenderSelectedAddons();
        }
        private void RecalculateAddonTotal()
        {
            _addonTotal = _addonList.Sum(x => x.TotalPrice);

            RecalculatePricing();
        }
        private void RenderSelectedAddons()
        {
            spSelectedAddons.Children.Clear();

            txtAddonCount.Text = $"{_addonList.Count} service(s)";
            txtAddonTotalAmount.Text = $"{_addonTotal:N0} ₫";

            if (_addonList.Count == 0)
            {
                TextBlock emptyText = new TextBlock
                {
                    Text = "No services selected yet.\nBrowse services on the left and add them.",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8A7D77")),
                    FontSize = 13,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 30, 0, 0)
                };
                spSelectedAddons.Children.Add(emptyText);
                return;
            }

            foreach (var addon in _addonList)
            {
                Border itemBorder = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FAFAF8")),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(14),
                    Margin = new Thickness(0, 0, 0, 8),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1E7E2")),
                    BorderThickness = new Thickness(1)
                };

                Grid itemGrid = new Grid();
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Left: Info
                StackPanel infoPanel = new StackPanel();

                TextBlock txtServiceName = new TextBlock
                {
                    Text = addon.ServiceName,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 13,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3131"))
                };
                infoPanel.Children.Add(txtServiceName);

                TextBlock txtDetail = new TextBlock
                {
                    Text = $"{addon.NumberOfPeople} pax × {addon.PriceDisplay} = {addon.TotalPriceDisplay}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                    Margin = new Thickness(0, 3, 0, 0)
                };
                infoPanel.Children.Add(txtDetail);

                TextBlock txtDate = new TextBlock
                {
                    Text = addon.FromDateDisplay,
                    FontSize = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8A7D77")),
                    Margin = new Thickness(0, 2, 0, 0)
                };
                infoPanel.Children.Add(txtDate);

                if (!string.IsNullOrWhiteSpace(addon.Notes))
                {
                    TextBlock txtNotes = new TextBlock
                    {
                        Text = $"📝 {addon.Notes}",
                        FontSize = 11,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8A7D77")),
                        Margin = new Thickness(0, 2, 0, 0),
                        TextWrapping = TextWrapping.Wrap
                    };
                    infoPanel.Children.Add(txtNotes);
                }

                Grid.SetColumn(infoPanel, 0);
                itemGrid.Children.Add(infoPanel);

                // Right: Remove button
                DTO_CreateBookingAddon capturedAddon = addon;
                Button btnRemove = new Button
                {
                    Content = "✕",
                    Width = 28,
                    Height = 28,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEE2E2")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626")),
                    BorderThickness = new Thickness(0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                    Cursor = Cursors.Hand,
                    VerticalAlignment = VerticalAlignment.Top
                };
                btnRemove.Click += (s, ev) =>
                {
                    RemoveAddon(capturedAddon);
                };
                Grid.SetColumn(btnRemove, 1);
                itemGrid.Children.Add(btnRemove);

                itemBorder.Child = itemGrid;
                spSelectedAddons.Children.Add(itemBorder);
            }
        }


        // STEP 5: CHI PHÍ (CŨ LÀ STEP 4)


        private void LoadStep5()
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
            txtDiscountAmount.Text = _discountAmount > 0
                ? $"-{_discountAmount:N0} ₫"
                : "0 ₫";
            txtServiceFee.Text = $"{_serviceFee:N0} ₫";
            txtTaxAmount.Text = $"{_taxAmount:N0} ₫";
            txtFinalAmount.Text = $"{_finalAmount:N0} ₫";

            // Hiển thị dòng Addon Services (nếu có)
            if (_addonList.Any())
            {
                AddonTotalRow.Visibility = Visibility.Visible;
                txtAddonServicesLine.Text = $"+{_addonTotal:N0} ₫ ({_addonList.Count} dịch vụ)";
            }
            else
            {
                AddonTotalRow.Visibility = Visibility.Collapsed;
            }

            RefreshCouponUI();
        }


        // STEP 6: THANH TOÁN


        private void LoadStep6()
        {
            txtReviewGuestName.Text = _selectedGuest.FullName;
            txtReviewGuestEmail.Text = _selectedGuest.Email;
            txtReviewGuestPhone.Text = _selectedGuest.PhoneNumber;
            txtReviewListingTitle.Text = _selectedListing.Title;
            txtReviewListingArea.Text = _selectedListing.ApproximateAddress;
            txtReviewDates.Text = $"{dpCheckIn.SelectedDate:dd/MM/yyyy} - {dpCheckOut.SelectedDate:dd/MM/yyyy}";
            txtReviewGuests.Text = $"{txtGuests.Text} Guest(s)";
            txtReviewPolicy.Text = (cbPolicy.SelectedItem as ET_CancellationPolicies)?.Name ?? "";
            txtReviewFinalAmount.Text = $"{_finalAmount:N0} ₫";

            if (_appliedCoupon != null)
            {
                ReviewCouponPanel.Visibility = Visibility.Visible;
                txtReviewCoupon.Text = $"{_appliedCoupon.CouponCode}  (-{_discountAmount:N0} ₫)";
            }
            else
            {
                ReviewCouponPanel.Visibility = Visibility.Collapsed;
            }

            // Hiển thị Addon Services Review (nếu có)
            if (_addonList.Any())
            {
                ReviewAddonPanel.Visibility = Visibility.Visible;

                StringBuilder sb = new StringBuilder();
                foreach (var addon in _addonList)
                {
                    sb.AppendLine($"• {addon.ServiceName} — {addon.NumberOfPeople} pax — {addon.FromDateDisplay}");
                }
                txtReviewAddonSummary.Text = sb.ToString().TrimEnd();
                txtReviewAddonTotal.Text = $"Addon Total: {_addonTotal:N0} ₫";
            }
            else
            {
                ReviewAddonPanel.Visibility = Visibility.Collapsed;
            }
        }


        // PRICING (CẬP NHẬT — CỘNG ADDON)


        private void RecalculatePricing()
        {
            if (_appliedCoupon != null)
                _discountAmount = _couponBus.CalculateDiscount(_baseAmount, _appliedCoupon);
            else
                _discountAmount = 0;

            // Tính addon total
            _addonTotal = _addonList.Sum(x => x.TotalPrice);

            _finalAmount = _baseAmount - _discountAmount + _serviceFee + _taxAmount + _addonTotal;

            if (_finalAmount < 0)
                _finalAmount = 0;
        }


        // COUPON


        private void btnApplyCoupon_Click(object sender, RoutedEventArgs e)
        {
            string code = txtCouponCode.Text?.Trim();

            string error = _couponBus.ValidateCoupon(code, out ET_Coupons coupon);

            if (error != null)
            {
                MessageBox.Show(error, "Coupon", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _appliedCoupon = coupon;

            RecalculatePricing();

            txtDiscountAmount.Text = _discountAmount > 0
                ? $"-{_discountAmount:N0} ₫"
                : "0 ₫";
            txtFinalAmount.Text = $"{_finalAmount:N0} ₫";

            RefreshCouponUI();
        }
        private void btnRemoveCoupon_Click(object sender, RoutedEventArgs e)
        {
            _appliedCoupon = null;

            RecalculatePricing();

            txtCouponCode.Text = "";
            txtDiscountAmount.Text = "0 ₫";
            txtFinalAmount.Text = $"{_finalAmount:N0} ₫";

            RefreshCouponUI();
        }
        private void RefreshCouponUI()
        {
            if (_appliedCoupon != null)
            {
                CouponInputPanel.Visibility = Visibility.Collapsed;
                CouponAppliedPanel.Visibility = Visibility.Visible;

                txtCouponAppliedCode.Text = $"✓ {_appliedCoupon.CouponCode}";

                string detail = $"-{_appliedCoupon.DiscountPercent}%";
                if (_appliedCoupon.MaximumDiscountAmount > 0)
                    detail += $", max {_appliedCoupon.MaximumDiscountAmount:N0} ₫";
                detail += $" → saved {_discountAmount:N0} ₫";

                txtCouponAppliedDetail.Text = detail;
            }
            else
            {
                CouponInputPanel.Visibility = Visibility.Visible;
                CouponAppliedPanel.Visibility = Visibility.Collapsed;
            }
        }


        // PAYMENT (STEP 6)


        private void cbPaymentStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPaymentStatus.SelectedItem == null)
                return;

            string status = (cbPaymentStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();

            bool needsTransaction = status == "Paid" || status == "Deposit";

            if (TransactionTypePanel != null)
                TransactionTypePanel.Visibility = needsTransaction
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (DepositPanel != null)
                DepositPanel.Visibility = status == "Deposit"
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private bool ValidateStep6()
        {
            if (cbPaymentStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select payment status.");
                return false;
            }

            string status = (cbPaymentStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (status == "Paid" || status == "Deposit")
            {
                if (cbTransactionType.SelectedValue == null)
                {
                    MessageBox.Show("Please select a payment method.");
                    return false;
                }
            }

            if (status == "Deposit")
            {
                if (!decimal.TryParse(txtDepositAmount.Text, out decimal deposit) || deposit <= 0)
                {
                    MessageBox.Show("Please enter a valid deposit amount.");
                    return false;
                }

                if (deposit >= _finalAmount)
                {
                    MessageBox.Show($"Deposit amount must be less than the total ({_finalAmount:N0} ₫).");
                    return false;
                }
            }

            return true;
        }


        // NAVIGATION


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
            // Step 4 (Addon): Không bắt buộc validate — addon là tùy chọn
            // Chỉ recalculate pricing trước khi sang Step 5
            if (_currentStep == 4)
            {
                RecalculatePricing();
            }
            if (_currentStep == 6)
            {
                if (!ValidateStep6())
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

                var result = _bookingBus.CreateManualBooking(dto);

                if (!result.Success)
                {
                    MessageBox.Show(result.Error, "Create Booking", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // ─── INVOICE + QR + PDF + SIGNALR ───

                bool invoiceShown = false;

                if (_invoiceBus.CanGenerateInvoice(result.BookingId))
                {
                    string invoiceError;
                    long invoiceId = _invoiceBus.GenerateInvoice(result.BookingId, out invoiceError);

                    if (invoiceId > 0)
                    {
                        string baseUrl = SignalRService.Instance.PaymentBaseUrl;
                        DTO_Invoice invoice = _invoiceBus.GetInvoiceFull(invoiceId, baseUrl);

                        if (invoice != null)
                        {
                            try
                            {
                                // 1. Generate QR Code
                                BitmapImage qrBitmap = Helper_QRCode.GenerateBitmapImage(invoice.PaymentUrl);
                                byte[] qrPngBytes = Helper_QRCode.GeneratePngBytes(invoice.PaymentUrl);

                                // 2. Export PDF ra Desktop
                                string pdfFileName = $"Invoice_{invoice.InvoiceCode}.pdf";
                                string pdfPath = System.IO.Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                    pdfFileName);

                                Helper_InvoicePDF.ExportPdf(invoice, qrPngBytes, pdfPath);

                                // 3. Connect SignalR (fire-and-forget, không block UI)
                                ConnectSignalRAsync();

                                // 4. Show Invoice Dialog
                                var dialog = new InvoiceGeneratedDialog(
                                    qrBitmap,
                                    pdfPath,
                                    invoiceId,
                                    invoice.InvoiceCode,
                                    invoice.TotalAmountDisplay);

                                dialog.Owner = this;
                                dialog.ShowDialog();

                                invoiceShown = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(
                                    $"Booking created but failed to generate invoice:\n{ex.Message}",
                                    "Invoice Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(invoiceError))
                    {
                        MessageBox.Show(
                            $"Booking created, but invoice could not be generated:\n{invoiceError}",
                            "Invoice Warning",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }

                if (!invoiceShown)
                {
                    MessageBox.Show(
                        "Booking created successfully.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
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


        // BUILD DTO (CẬP NHẬT — THÊM ADDON LIST)


        private DTO_CreateBooking BuildBookingDto()
        {
            string status = (cbPaymentStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Pending";

            bool isPaid = status == "Paid";
            bool isDeposit = status == "Deposit";

            decimal depositAmount = 0;
            if (isDeposit)
                decimal.TryParse(txtDepositAmount.Text, out depositAmount);

            long transactionTypeId = 0;
            if ((isPaid || isDeposit) && cbTransactionType.SelectedValue != null)
                transactionTypeId = (long)cbTransactionType.SelectedValue;

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
                AddonList = _addonList,
                CouponID = _appliedCoupon?.ID,
                CouponCode = _appliedCoupon?.CouponCode,
                CreatedByUserID = _currentUser.ID,
                PaymentStatus = status,
                IsPaid = isPaid,
                IsDeposit = isDeposit,
                DepositAmount = depositAmount,
                TransactionTypeID = transactionTypeId
            };
        }
        /// <summary>
        /// Kết nối SignalR (fire-and-forget).
        /// Nếu fail → bỏ qua, user vẫn có QR + PDF.
        /// </summary>
        private async void ConnectSignalRAsync()
        {
            try
            {
                await SignalRService.Instance.ConnectAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SignalR] Connection failed: {ex.Message}");
            }
        }
    }
}