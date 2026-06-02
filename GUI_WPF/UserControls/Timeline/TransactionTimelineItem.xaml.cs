using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Timeline
{
    public partial class TransactionTimelineItem : UserControl
    {
        public TransactionTimelineItem()
        {
            InitializeComponent();
        }

         
        // TITLE
         

        public string Title
        {
            get => txtTitle.Text;
            set => txtTitle.Text = value;
        }

         
        // SUBTITLE
         

        public string Subtitle
        {
            get => txtSubtitle.Text;
            set => txtSubtitle.Text = value;
        }

         
        // DATE
         

        public string Date
        {
            get => txtDate.Text;
            set => txtDate.Text = value;
        }

         
        // TYPE
         

        public string TransactionType
        {
            get => txtType.Text;
            set => txtType.Text = value;
        }

         
        // AMOUNT
         

        public string Amount
        {
            get => txtAmount.Text;
            set => txtAmount.Text = value;
        }

         
        // STATUS
         

        public string Status
        {
            get => txtStatus.Text;
            set
            {
                txtStatus.Text = value;

                switch (value)
                {
                    case "Completed":

                        ApplyStatusTheme(
                            "#E8F8F0",
                            "#47A87B");

                        break;

                    case "Pending":

                        ApplyStatusTheme(
                            "#FFF6E7",
                            "#D29B22");

                        break;

                    case "Failed":

                        ApplyStatusTheme(
                            "#FDECEC",
                            "#D96A6A");

                        break;

                    case "Refunded":

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

         
        // POSITIVE / NEGATIVE
         

        public bool IsPositive
        {
            set
            {
                if (value)
                {
                    txtAmount.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#47A87B"));
                }
                else
                {
                    txtAmount.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D96A6A"));
                }
            }
        }

         
        // ICON THEMES
         

        public void SetPaymentTheme()
        {
            IconBubble.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#EEF3FF"));

            txtIcon.Text = "💳";
        }

        public void SetRefundTheme()
        {
            IconBubble.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FDECEC"));

            txtIcon.Text = "↩";
        }

        public void SetPayoutTheme()
        {
            IconBubble.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#E8F8F0"));

            txtIcon.Text = "💸";
        }

        public void SetCouponTheme()
        {
            IconBubble.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FFF6E7"));

            txtIcon.Text = "🎟";
        }

         
        // STATUS THEME
         

        private void ApplyStatusTheme(
            string background,
            string foreground)
        {
            StatusChip.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(background));

            txtStatus.Foreground =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(foreground));
        }
    }
}
