using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Cards
{
    public partial class UserBookingCard : UserControl
    {
        public UserBookingCard()
        {
            InitializeComponent();
        }

        // EVENTS

        public event RoutedEventHandler ViewClicked;

        public event RoutedEventHandler ActionClicked;

         
        // PROPERTY

        public string PropertyTitle
        {
            get => txtProperty.Text;
            set => txtProperty.Text = value;
        }

        public string Area
        {
            get => txtArea.Text;
            set => txtArea.Text = value;
        }

        // GUEST
        public string GuestName
        {
            get => txtGuest.Text;
            set
            {
                txtGuest.Text = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] parts = value.Split(' ');

                    if (parts.Length >= 2)
                    {
                        txtInitials.Text =
                            $"{parts[0][0]}{parts[1][0]}";
                    }
                }
            }
        }

         
        // BOOKING STATUS
        public string Status
        {
            get => txtStatus.Text;
            set
            {
                txtStatus.Text = value;

                switch (value)
                {
                    case "Confirmed":

                        ApplyStatusTheme(
                            "#E8F8F0",
                            "#47A87B");

                        break;

                    case "Pending":

                        ApplyStatusTheme(
                            "#FFF6E7",
                            "#D29B22");

                        break;

                    case "Cancelled":

                        ApplyStatusTheme(
                            "#FDECEC",
                            "#D96A6A");

                        break;

                    case "Completed":

                        ApplyStatusTheme(
                            "#EEF3FF",
                            "#6484D6");

                        break;

                    default:

                        ApplyStatusTheme(
                            "#F4F4F4",
                            "#777777");

                        break;
                }
            }
        }

         
        // DATES
        public string StayDates
        {
            get => txtDates.Text;
            set => txtDates.Text = value;
        }

        // PRICE

        public string Price
        {
            get => txtPrice.Text;
            set => txtPrice.Text = value;
        }

         
        // DETAILS
        public string GuestCount
        {
            get => txtGuests.Text;
            set => txtGuests.Text = value;
        }

        public string Nights
        {
            get => txtNights.Text;
            set => txtNights.Text = value;
        }

        // BOOKING ID
        public string BookingID
        {
            get => txtBookingID.Text;
            set => txtBookingID.Text = value;
        }

         
        // ACTION BUTTON
        public string ActionText
        {
            get => btnAction.Content.ToString();
            set => btnAction.Content = value;
        }

        // STATUS THEME
        private void ApplyStatusTheme(string background, string foreground)
        {
            StatusChip.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(background));

            txtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foreground));
        }

        // EVENTS
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            ViewClicked?.Invoke(this, e);
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            ActionClicked?.Invoke(this, e);
        }
    }
}
