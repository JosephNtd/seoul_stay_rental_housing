using DTO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Cards
{
    public partial class BookingTimelineItem : UserControl
    {
        public BookingTimelineItem()
        {
            InitializeComponent();
        }

        public DTO_BookingTimeline Timeline { get; private set; }
        public bool IsLastItem { get; set; }

        public void LoadTimeline(DTO_BookingTimeline timeline, bool isLast = false)
        {
            if (timeline == null)
                return;

            Timeline = timeline;
            IsLastItem = isLast;
            txtStatus.Text = timeline.NewStatus;
            txtDate.Text = timeline.ChangedDate.ToString("dd MMM yyyy HH:mm");
            txtNote.Text = string.IsNullOrWhiteSpace(timeline.Notes) ? "-" : timeline.Notes;
            txtType.Text = GetTypeText(timeline.NewStatus);
            ApplyStatusStyle(timeline.NewStatus);
            LineBelow.Visibility = isLast ? Visibility.Collapsed : Visibility.Visible;
        }

        private string GetTypeText(
            string status)
        {
            switch (status)
            {
                case "Pending":
                    return "Created";

                case "Confirmed":
                    return "Confirmation";

                case "CheckedIn":
                    return "Check-In";

                case "Completed":
                    return "Check-Out";

                case "Cancelled":
                    return "Cancellation";

                case "Refunded":
                    return "Refund";

                default:
                    return "Status Change";
            }
        }

        private void ApplyStatusStyle(
            string status)
        {
            switch (status)
            {
                case "Pending":
                    StatusDot.Fill = Brushes.Goldenrod;
                    TypeBadge.Background = Brushes.LemonChiffon;
                    break;

                case "Confirmed":
                    StatusDot.Fill = Brushes.SteelBlue;
                    TypeBadge.Background = Brushes.LightBlue;
                    break;

                case "CheckedIn":
                    StatusDot.Fill = Brushes.SeaGreen;
                    TypeBadge.Background = Brushes.Honeydew;
                    break;

                case "Completed":
                    StatusDot.Fill = Brushes.MediumPurple;
                    TypeBadge.Background = Brushes.Lavender;
                    break;

                case "Cancelled":
                    StatusDot.Fill = Brushes.IndianRed;
                    TypeBadge.Background = Brushes.MistyRose;
                    break;

                case "Refunded":
                    StatusDot.Fill = Brushes.DarkOrange;
                    TypeBadge.Background = Brushes.Bisque;
                    break;

                default:
                    StatusDot.Fill = Brushes.Gray;
                    TypeBadge.Background = Brushes.Gainsboro;
                    break;
            }
        }
    }
}