using DTO;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI_WPF.UserControls.Cards
{
    public partial class BookingCard : UserControl
    {
        public BookingCard()
        {
            InitializeComponent();
        }

        public DTO_BookingCard Booking
        {
            get;
            private set;
        }

        public event EventHandler OpenClicked;
        public event EventHandler ConfirmClicked;
        public event EventHandler CheckInClicked;
        public event EventHandler CheckOutClicked;
        public event EventHandler CancelClicked;

        public void LoadBooking(
            DTO_BookingCard booking)
        {
            if (booking == null)
                return;

            Booking = booking;
            txtBookingCode.Text = booking.BookingCode;
            txtBookingDate.Text = booking.BookingDateDisplay;
            txtGuestName.Text = booking.GuestFullName;
            txtGuestEmail.Text = booking.GuestEmail;
            txtListing.Text = booking.ListingTitle;
            txtDates.Text = $"{booking.CheckInDate:dd MMM yyyy} → {booking.CheckOutDate:dd MMM yyyy}";
            txtGuests.Text = $"{booking.NumberOfGuests} guest(s)";
            txtPrice.Text = booking.FinalPriceDisplay;
            txtStatus.Text = booking.BookingStatus;

            LoadStatus();

            LoadAvatar();

            btnConfirm.Visibility = booking.BookingStatus == "Pending" ? Visibility.Visible : Visibility.Collapsed;

            btnCheckIn.Visibility = booking.BookingStatus == "Confirmed" ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadAvatar()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Booking.GuestAvatar))
                {
                    imgGuest.Source = new BitmapImage(new Uri(Booking.GuestAvatar, UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadStatus()
        {
            switch (Booking.BookingStatus)
            {
                case "Pending":
                    StatusBadge.Background = new SolidColorBrush(Color.FromRgb(255, 244, 214));
                    break;

                case "Confirmed":
                    StatusBadge.Background = new SolidColorBrush(Color.FromRgb(219, 234, 254));
                    break;

                case "CheckedIn":
                    StatusBadge.Background = new SolidColorBrush(Color.FromRgb(220, 252, 231));
                    break;

                case "Completed":
                    StatusBadge.Background = new SolidColorBrush(Color.FromRgb(237, 233, 254));
                    break;

                case "Cancelled":
                    StatusBadge.Background = new SolidColorBrush(Color.FromRgb(254, 226, 226));
                    break;
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            ConfirmClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            CheckInClicked?.Invoke(this, EventArgs.Empty);
        }

        private void miCheckOut_Click(object sender, RoutedEventArgs e)
        {
            CheckOutClicked?.Invoke(this, EventArgs.Empty);
        }

        private void miCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            btnMore.ContextMenu.IsOpen = true;
        }
    }
}