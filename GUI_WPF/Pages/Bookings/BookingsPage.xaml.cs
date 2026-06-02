using BUS;
using DTO;
using ET;
using GUI_WPF.UserControls.Cards;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.Pages.Bookings
{
    public partial class BookingsPage : Page
    {
        private readonly BUS_Booking _bookingBus = new BUS_Booking();

        private List<DTO_BookingCard> _bookings = new List<DTO_BookingCard>();
        private readonly ET_Users _currentUser;
        public BookingsPage(ET_Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            Loaded += BookingsPage_Loaded;
        }

        private void BookingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPage();
        }

        private void LoadPage()
        {
            LoadStats();

            LoadBookings();
        }

        private void LoadStats()
        {
            bookingStatsPanel.LoadStats(
                _bookingBus.GetBookingStats());
        }

        private void LoadBookings()
        {
            _bookings = _bookingBus.GetBookingCards();

            RenderBookings(_bookings);
        }

        private void RenderBookings(IEnumerable<DTO_BookingCard> data)
        {
            wpBookings.Children.Clear();

            foreach (var booking in data)
            {
                var card = new BookingCard();

                card.Margin = new Thickness(0, 0, 16, 16);
                card.LoadBooking(booking);

                card.OpenClicked += Card_OpenClicked;

                card.ConfirmClicked += Card_ConfirmClicked;

                card.CheckInClicked += Card_CheckInClicked;

                card.CheckOutClicked += Card_CheckOutClicked;

                card.CancelClicked += Card_CancelClicked;

                wpBookings.Children.Add(card);
            }
        }

        private void Card_OpenClicked(object sender, System.EventArgs e)
        {
            var card = sender as BookingCard;

            if (card == null)
                return;

            var detail = _bookingBus.GetBookingDetails(card.Booking.BookingID);

            var window = new BookingWindow(detail);

            bool? result = window.ShowDialog();

            if (result == true)
            {
                LoadPage();
            }
        }

        private void Card_ConfirmClicked(object sender, System.EventArgs e)
        {
            var card = sender as BookingCard;

            if (card == null)
                return;

            bool success = _bookingBus.ConfirmBooking(card.Booking.BookingID, _currentUser.ID);

            if (!success)
            {
                MessageBox.Show("Unable to confirm booking.");
                return;
            }

            LoadPage();
        }

        private void Card_CheckInClicked(object sender, System.EventArgs e)
        {
            var card = sender as BookingCard;

            if (card == null)
                return;

            bool success = _bookingBus.CheckInBooking(card.Booking.BookingID, _currentUser.ID);

            if (!success)
            {
                MessageBox.Show("Unable to check in.");
                return;
            }

            LoadPage();
        }

        private void Card_CheckOutClicked(object sender, System.EventArgs e)
        {
            var card = sender as BookingCard;

            if (card == null)
                return;
            bool success = _bookingBus.CheckOutBooking(card.Booking.BookingID, _currentUser.ID);

            if (!success)
            {
                MessageBox.Show("Unable to check out.");
                return;
            }

            LoadPage();
        }

        private void Card_CancelClicked(object sender, System.EventArgs e)
        {
            var card = sender as BookingCard;

            if (card == null)
                return;

            bool success = _bookingBus.CancelBooking(card.Booking.BookingID, _currentUser.ID, "Cancelled by manager");

            if (!success)
            {
                MessageBox.Show("Unable to cancel booking.");
                return;
            }

            LoadPage();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<DTO_BookingCard> result = _bookings;

            string keyword = txtSearch.Text?.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(x =>  (x.BookingCode ?? "").ToLower().Contains(keyword)   ||
                                            (x.GuestFullName ?? "").ToLower().Contains(keyword) ||
                                            (x.ListingTitle ?? "").ToLower().Contains(keyword));
            }

            var statusItem = cbStatus.SelectedItem as ComboBoxItem;

            if (statusItem != null)
            {
                string status = statusItem.Content.ToString();

                if (status != "All")
                {
                    result = result.Where(x => x.BookingStatus == status);
                }
            }

            RenderBookings(result);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPage();
        }

        private void btnNewBooking_Click(object sender, RoutedEventArgs e)
        {
            var win = new CreateBookingWindowWizard(_currentUser);

            bool? result = win.ShowDialog();

            if (result == true)
            {
                LoadBookings();
            }
        }
    }
}