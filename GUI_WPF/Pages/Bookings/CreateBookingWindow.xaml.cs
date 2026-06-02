using BUS;
using DTO;
using ET;
using GUI_WPF.UserControls.Cards;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for CreateBookingWindow.xaml
    /// </summary>
    public partial class CreateBookingWindow : Window
    {
        private readonly BUS_Guest _guestBus = new BUS_Guest();
        private readonly BUS_Items _itemBus = new BUS_Items();
        private readonly BUS_Booking _bookingBus = new BUS_Booking();
        private readonly BUS_CancellationPolicy _policyBus = new BUS_CancellationPolicy();
        private readonly BUS_TransactionType _transactionType = new BUS_TransactionType();
        private readonly BUS_ItemPrices _priceBus = new BUS_ItemPrices();
        private DTO_GuestLookup _selectedGuest;
        private DTO_ItemCard _selectedListing;
        private List<DTO_CreateBookingNight> _nights = new List<DTO_CreateBookingNight>();
        private readonly ET_Users _currentUser;
        private decimal _baseAmount;
        private decimal _discountAmount;
        private decimal _cleaningFee;
        private decimal _serviceFee;
        private decimal _taxAmount;
        private decimal _finalAmount;

        public CreateBookingWindow(ET_Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            Loaded += CreateBookingWindow_Loaded;
            dpCheckIn.SelectedDateChanged += ResetAvailability;
            dpCheckOut.SelectedDateChanged += ResetAvailability;
        }
        private void ResetAvailability(object sender, SelectionChangedEventArgs e)
        {
            AvailabilityChip.Text = "Not Checked";
            AvailabilityChip.Status = "draft";
        }

        private void CreateBookingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGuests();
            LoadListings();
            LoadPolicies();
            LoadPaymentStatus();
            ResetSummary();

            DepositPanel.Visibility = Visibility.Collapsed;
            AvailabilityChip.Text = "Not Checked";
            AvailabilityChip.Status = "draft";
        }
        private void LoadGuests()
        {
            dgGuests.ItemsSource = _guestBus.GetLookupData();
        }
        private void LoadListings()
        {
            wpListings.Children.Clear();

            var listings = _itemBus.GetItemCards();

            foreach (var item in listings)
            {
                var card = new ListingCard();

                card.LoadListing(item);

                card.OpenClicked += (s, e) =>
                    {
                        SelectListing(item);
                    };

                wpListings.Children.Add(card);
            }
        }
        private void SelectListing(DTO_ItemCard item)
        {
            _selectedListing = item;

            ListingEmptyState.Visibility = Visibility.Collapsed;

            SelectedListingPanel.Visibility = Visibility.Visible;

            txtSelectedListing.Text = item.Title + Environment.NewLine + item.AreaName;
        }
        private void LoadPolicies()
        {
            cbPolicy.ItemsSource = _policyBus.GetAll();

            cbPolicy.DisplayMemberPath = "Name";

            cbPolicy.SelectedValuePath = "ID";
        }
        private void LoadPaymentStatus()
        {
            cbPaymentStatus.ItemsSource = _transactionType.GetData();

            cbPaymentStatus.DisplayMemberPath = "Name";

            cbPaymentStatus.SelectedValuePath = "ID";
        }
        private void cbPaymentStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPaymentStatus.SelectedItem == null)
                return;

            string status = (cbPaymentStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();
            DepositPanel.Visibility = status == "Deposit" ? Visibility.Visible : Visibility.Collapsed;
        }
        private void dgGuests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgGuests.SelectedItem == null)
                return;

            _selectedGuest = dgGuests.SelectedItem as DTO_GuestLookup;

            GuestEmptyState.Visibility = Visibility.Collapsed;

            SelectedGuestPanel.Visibility = Visibility.Visible;

            txtSelectedGuest.Text =
                $"{_selectedGuest.FullName}\n" +
                $"{_selectedGuest.Email}\n" +
                $"{_selectedGuest.PhoneNumber}";
        }
        private void btnCheckAvailability_Click(object sender, RoutedEventArgs e)
        {
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
            if (dpCheckOut.SelectedDate.Value <= dpCheckIn.SelectedDate.Value)
            {
                MessageBox.Show("Check-out date must be after check-in date.");
                return;
            }

            bool available = _bookingBus.ValidateDateOverlap(_selectedListing.ID, dpCheckIn.SelectedDate.Value, dpCheckOut.SelectedDate.Value);

            if (!available)
            {
                AvailabilityChip.Text = "Already Booked";
                AvailabilityChip.Status = "cancelled";

                return;
            }

            AvailabilityChip.Text = "Available";

            AvailabilityChip.Status = "confirmed";

            GeneratePricing();
        }
        private void GeneratePricing()
        {
            _baseAmount = 0;
            _nights.Clear();

            spNightCards.Children.Clear();

            var allPrices = _priceBus.GetPrices(_selectedListing.ID);

            DateTime current = dpCheckIn.SelectedDate.Value;

            DateTime checkOut = dpCheckOut.SelectedDate.Value;

            while (current < checkOut)
            {
                var itemPrice = allPrices.FirstOrDefault(x => x.Date.Date == current.Date);

                if (itemPrice == null)
                {
                    MessageBox.Show($"No pricing found for {current:dd/MM/yyyy}");
                    return;
                }

                var night =
                    new DTO_CreateBookingNight
                    {
                        Date = current,
                        ItemPriceID = itemPrice.ID,
                        Price = itemPrice.Price,
                        CancellationPolicyID = itemPrice.CancellationPolicyID
                    };

                _nights.Add(night);
                _baseAmount += night.Price;
                current = current.AddDays(1);
            }

            RenderNightCards();

            CalculateSummary();

            PricingEmptyState.Visibility = Visibility.Collapsed;
            PricingScrollViewer.Visibility = Visibility.Visible;
        }
        private void RenderNightCards()
        {
            spNightCards.Children.Clear();

            foreach (var night in _nights)
            {
                var card = new BookingNightCard();
                card.LoadNight(night);
                spNightCards.Children.Add(card);
            }
        }
        private void CalculateSummary()
        {
            _discountAmount = 0;

            _cleaningFee = 0;

            _serviceFee = _baseAmount * 0.05m;

            _taxAmount = _baseAmount * 0.10m;

            _finalAmount = _baseAmount - _discountAmount + _cleaningFee + _serviceFee + _taxAmount;

            txtBaseAmount.Text = _baseAmount.ToString("N0");

            txtDiscountAmount.Text = _discountAmount.ToString("N0");

            txtCleaningFee.Text = _cleaningFee.ToString("N0");

            txtServiceFee.Text = _serviceFee.ToString("N0");

            txtTaxAmount.Text = _taxAmount.ToString("N0");

            txtFinalAmount.Text = _finalAmount.ToString("N0");
        }
        private void btnCreateBooking_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGuest == null)
            {
                MessageBox.Show("Please select guest.");

                return;
            }

            if (_selectedListing == null)
            {
                MessageBox.Show("Please select listing.");

                return;
            }
            if (!int.TryParse(txtGuests.Text, out int guestCount))
            {
                MessageBox.Show("Invalid guest count.");
                return;
            }
            decimal deposit = 0;

            if (!string.IsNullOrWhiteSpace(txtDeposit.Text))
            {
                if (!decimal.TryParse(txtDeposit.Text, out deposit))
                {
                    MessageBox.Show("Invalid deposit amount.");
                    return;
                }
            }
            if (cbPolicy.SelectedValue == null)
            {
                MessageBox.Show("Please select cancellation policy.");
                return;
            }
            if (guestCount > _selectedListing.Capacity)
            {
                MessageBox.Show($"Maximum capacity is {_selectedListing.Capacity}");
                return;
            }
            var dto =
                new DTO_CreateBooking
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
                    SpecialRequests = txtSpecialRequests.Text,
                    BaseAmount = _baseAmount,
                    DiscountAmount = _discountAmount,
                    CleaningFee = _cleaningFee,
                    ServiceFee = _serviceFee,
                    TaxAmount = _taxAmount,
                    FinalAmount = _finalAmount,
                    Nights = _nights,
                    CreatedByUserID = _currentUser.ID,
                    IsPaid = cbPaymentStatus.Text == "Paid",
                    IsDeposit = cbPaymentStatus.Text == "Deposit",
                    DepositAmount = string.IsNullOrWhiteSpace(txtDeposit.Text) ? 0 : decimal.Parse(txtDeposit.Text),
                };

            string error;
            bool success = _bookingBus.CreateManualBooking(dto, out error);

            if (!success)
            {
                MessageBox.Show(error);
                return;
            }

            MessageBox.Show($"Booking created successfully.\n\n" + $"Total Amount: {_finalAmount:N0} ₫");

            DialogResult = true;

            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ResetSummary()
        {
            txtBaseAmount.Text = "0";
            txtDiscountAmount.Text = "0";
            txtCleaningFee.Text = "0";
            txtServiceFee.Text = "0";
            txtTaxAmount.Text = "0";
            txtFinalAmount.Text = "0";
        }
        private void btnNewGuest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Quick guest creation will be implemented later.");
        }
    }
}
