using BUS;
using DTO;
using GUI_WPF.UserControls.Cards;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI_WPF.Pages.Bookings
{
    public partial class BookingWindow : Window
    {
        private readonly BUS_Booking _bookingBus = new BUS_Booking();

        private DTO_BookingDetails _booking;

        public BookingWindow(DTO_BookingDetails booking)
        {
            InitializeComponent();

            _booking = booking;

            LoadData();
        }

        private void LoadData()
        {
            if (_booking == null)
                return;

            LoadHeader();

            LoadOverview();

            LoadPayment();

            LoadTimeline();

            LoadNights();

            LoadActions();
        }

        private void LoadHeader()
        {
            txtBookingCode.Text = _booking.BookingCode;

            txtBookingDate.Text = _booking.BookingDateDisplay;

            txtStatus.Text = _booking.BookingStatus;

            LoadStatusBadge();
        }

        private void LoadStatusBadge()
        {
            switch (_booking.BookingStatus)
            {
                case "Pending":

                    StatusBadge.Background = Brushes.LemonChiffon;

                    txtStatus.Foreground = Brushes.DarkOrange;

                    break;

                case "Confirmed":

                    StatusBadge.Background = Brushes.LightBlue;

                    txtStatus.Foreground = Brushes.DarkBlue;

                    break;

                case "CheckedIn":

                    StatusBadge.Background = Brushes.Honeydew;

                    txtStatus.Foreground = Brushes.SeaGreen;

                    break;

                case "Completed":

                    StatusBadge.Background = Brushes.Lavender;

                    txtStatus.Foreground = Brushes.MediumPurple;

                    break;

                case "Cancelled":

                    StatusBadge.Background = Brushes.MistyRose;

                    txtStatus.Foreground = Brushes.Firebrick;

                    break;
            }
        }

        private void LoadOverview()
        {
            txtGuestName.Text = _booking.GuestFullName;

            txtGuestEmail.Text = _booking.GuestEmail;

            txtGuestPhone.Text = _booking.GuestPhone;

            txtGuestCountry.Text = _booking.GuestCountry;

            txtListingTitle.Text = _booking.ListingTitle;

            txtListingType.Text = _booking.ListingType;

            txtArea.Text = _booking.AreaName;

            txtAddress.Text = _booking.ListingAddress;

            txtCheckIn.Text = _booking.CheckInDate.ToShortDateString();

            txtCheckOut.Text = _booking.CheckOutDate.ToShortDateString();

            txtGuests.Text = _booking.NumberOfGuests.ToString();

            txtNights.Text = _booking.TotalNights.ToString();

            txtSpecialRequests.Text = string.IsNullOrWhiteSpace(_booking.SpecialRequests) ? "-" : _booking.SpecialRequests;

            txtPricePerNight.Text = _booking.PricePerNightDisplay;

            txtTotalPrice.Text = _booking.TotalPrice.ToString();

            txtDiscount.Text = _booking.DiscountDisplay;

            txtCleaningFee.Text = _booking.CleaningFeeDisplay;

            txtServiceFee.Text = _booking.ServiceFeeDisplay;

            txtTax.Text = _booking.TaxDisplay;

            txtFinalPrice.Text = _booking.FinalPriceDisplay;

            LoadImages();
        }

        private void LoadImages()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_booking.GuestAvatar))
                {
                    imgGuest.Source = new BitmapImage(new Uri(_booking.GuestAvatar, UriKind.RelativeOrAbsolute));
                }
            }
            catch
            {

            }

            try
            {
                if (!string.IsNullOrWhiteSpace(_booking.ListingThumbnail))
                {
                    imgListing.Source = new BitmapImage(new Uri(_booking.ListingThumbnail, UriKind.RelativeOrAbsolute));
                }
            }
            catch
            {

            }
        }

        private void LoadPayment()
        {
            txtTransactionCode.Text = $"Transaction: {_booking.TransactionCode}";
            txtTransactionDate.Text = $"Date: {_booking.TransactionDate}";
            txtTransactionAmount.Text = $"Amount: {_booking.FinalPriceDisplay}";
            txtGatewayReturn.Text = $"Gateway Ref: {_booking.GatewayReturnID}";
            txtPaymentStatus.Text = $"Status: {_booking.PaymentStatus}";
        }

        private void LoadTimeline()
        {
            spTimeline.Children.Clear();

            if (_booking.Timeline == null)
                return;

            for (int i = 0; i < _booking.Timeline.Count; i++)
            {
                var item = new BookingTimelineItem();
                item.LoadTimeline(_booking.Timeline[i], i == _booking.Timeline.Count - 1);
                spTimeline.Children.Add(item);
            }
        }

        private void LoadNights()
        {
            spNights.Children.Clear();

            if (_booking.Nights == null)
                return;

            foreach (var night in _booking.Nights)
            {
                var card = new BookingNightCard();
                card.LoadNight(night);
                spNights.Children.Add(card);
            }
        }

        private void LoadActions()
        {
            btnConfirm.Visibility = _booking.CanConfirm ? Visibility.Visible : Visibility.Collapsed;

            btnCheckIn.Visibility = _booking.CanCheckIn ? Visibility.Visible : Visibility.Collapsed;

            btnCheckOut.Visibility = _booking.CanCheckOut ? Visibility.Visible : Visibility.Collapsed;

            btnCancel.Visibility = _booking.CanCancel ? Visibility.Visible : Visibility.Collapsed;

            btnRefund.Visibility = _booking.CanRefund ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "Confirm this booking?",
                "Booking",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question)
                != MessageBoxResult.Yes)
                return;

            bool success = _bookingBus.ConfirmBooking(_booking.BookingID, 1);

            if (!success)
            {
                MessageBox.Show("Unable to confirm booking.");
                return;
            }

            DialogResult = true;
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            bool success = _bookingBus.CheckInBooking(_booking.BookingID, 1);

            if (!success)
            {
                MessageBox.Show("Unable to check-in.");
                return;
            }

            DialogResult = true;
        }

        private void btnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            bool success = _bookingBus.CheckOutBooking(_booking.BookingID, 1);

            if (!success)
            {
                MessageBox.Show("Unable to check-out.");
                return;
            }

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "Cancel this booking?",
                "Booking",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning)
                != MessageBoxResult.Yes)
                return;

            bool success = _bookingBus.CancelBooking(_booking.BookingID, 1, "Cancelled by manager");

            if (!success)
            {
                MessageBox.Show("Unable to cancel booking.");
                return;
            }

            DialogResult = true;
        }

        private void btnRefund_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Refund feature will be implemented later.");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}