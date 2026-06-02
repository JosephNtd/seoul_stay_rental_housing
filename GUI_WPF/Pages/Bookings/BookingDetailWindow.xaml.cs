using DTO;
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
    /// Interaction logic for BookingDetailWindow.xaml
    /// </summary>
    public partial class BookingDetailWindow : Window
    {

        // STATE


        private DTO_UserBookingSummary _booking;

        // CTOR
        public BookingDetailWindow(DTO_UserBookingSummary booking)
        {
            InitializeComponent();

            _booking = booking;

            Loaded += BookingDetailWindow_Loaded;
        }


        // LOAD


        private void BookingDetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooking();
        }


        // LOAD BOOKING


        private void LoadBooking()
        {
            if (_booking == null)
                return;

            // HEADER
            txtBookingTitle.Text = $"Booking #{_booking.BookingID}";

            txtBookingStatus.Text = _booking.BookingStatus;

            txtBookingDate.Text = $"Created {_booking.CheckInDate:dd MMM yyyy}";

            // PROPERTY
            txtPropertyName.Text = _booking.ItemTitle;

            txtAreaName.Text = _booking.AreaName;

            txtAddress.Text = _booking.Address;

            txtPropertyType.Text = _booking.ItemTypeName;

            txtGuests.Text = $"{_booking.GuestCount} Guests";

            // STAY
            txtCheckIn.Text = _booking.CheckInDate.ToString("dd MMM yyyy");

            txtCheckOut.Text = _booking.CheckOutDate.ToString("dd MMM yyyy");

            txtNights.Text = $"{_booking.Nights} Nights";

            // GUEST
            txtGuestName.Text = _booking.GuestName;

            txtGuestEmail.Text = _booking.GuestEmail;

            txtGuestPhone.Text = _booking.GuestPhone;

            // PAYMENT
            txtBasePrice.Text = $"{_booking.BasePrice:N0}$";

            txtCleaningFee.Text = $"{_booking.CleaningFee:N0}$";

            txtServiceFee.Text = $"{_booking.ServiceFee:N0}$";

            txtTotal.Text = $"{_booking.FinalPrice:N0}$";

            // TIMELINE
            RenderTimeline();

            // STATUS BUTTONS
            UpdateButtons();
        }


        // TIMELINE


        private void RenderTimeline()
        {
            spTimeline.Children.Clear();

            AddTimelineItem("Booking Created", "Reservation was created successfully");

            AddTimelineItem("Payment Confirmed", "Guest payment completed");

            AddTimelineItem("Host Notified", "Host received booking request");

            if (_booking.BookingStatus == "Completed")
            {
                AddTimelineItem("Stay Completed", "Guest checked out successfully");
            }
        }

        private void AddTimelineItem(string title, string subtitle)
        {
            Border item =
                new Border
                {
                    Background = System.Windows.Media.Brushes.White,

                    CornerRadius = new CornerRadius(18),

                    Padding = new Thickness(18),

                    Margin = new Thickness(0, 0, 0, 14)
                };

            StackPanel panel = new StackPanel();

            TextBlock txtTitle = new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                FontSize = 16
            };

            TextBlock txtSub =
                new TextBlock
                {
                    Text = subtitle,
                    Margin = new Thickness(0, 6, 0, 0),
                    Foreground = System.Windows.Media.Brushes.Gray
                };

            panel.Children.Add(txtTitle);

            panel.Children.Add(txtSub);

            item.Child = panel;

            spTimeline.Children.Add(item);
        }


        // BUTTON STATE


        private void UpdateButtons()
        {
            switch (_booking.BookingStatus)
            {
                case "Completed":
                    btnApprove.IsEnabled = false;
                    break;

                case "Cancelled":
                    btnApprove.IsEnabled = false;
                    btnCancelBooking.IsEnabled = false;
                    break;
            }
        }


        // ACTIONS


        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Booking approved.");
        }

        private void btnCancelBooking_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Booking cancelled.");
        }

        private void btnRefund_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Refund issued.");
        }


        // EXPORT


        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export PDF.");
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Print booking.");
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
