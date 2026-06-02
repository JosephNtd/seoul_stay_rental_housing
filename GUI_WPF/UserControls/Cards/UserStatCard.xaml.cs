using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Cards
{
    public partial class UserStatCard : UserControl
    {
        public UserStatCard()
        {
            InitializeComponent();
        }

         
        // TITLE
         

        public string Title
        {
            get => txtTitle.Text;
            set => txtTitle.Text = value;
        }

         
        // VALUE
         

        public string Value
        {
            get => txtValue.Text;
            set => txtValue.Text = value;
        }

         
        // SUBTITLE
         

        public string Subtitle
        {
            get => txtSubtitle.Text;
            set => txtSubtitle.Text = value;
        }

         
        // ICON
         

        public string Icon
        {
            get => txtIcon.Text;
            set => txtIcon.Text = value;
        }

         
        // TREND
         

        public string Trend
        {
            get => txtTrend.Text;
            set => txtTrend.Text = value;
        }

         
        // PERIOD
         

        public string Period
        {
            get => txtPeriod.Text;
            set => txtPeriod.Text = value;
        }

         
        // POSITIVE / NEGATIVE
         

        public bool IsPositiveTrend
        {
            set
            {
                if (value)
                {
                    TrendChip.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#E8F8F0"));

                    txtTrend.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#47A87B"));

                    txtTrendIcon.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#47A87B"));

                    txtTrendIcon.Text = "↗";
                }
                else
                {
                    TrendChip.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FDECEC"));

                    txtTrend.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D96A6A"));

                    txtTrendIcon.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D96A6A"));

                    txtTrendIcon.Text = "↘";
                }
            }
        }

         
        // THEME VARIANTS
         

        public void SetRevenueTheme()
        {
            IconBubble.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FFF0F0"));

            txtIcon.Text = "💰";
        }

        public void SetBookingTheme()
        {
            IconBubble.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#EEF3FF"));

            txtIcon.Text = "📅";
        }

        public void SetTransactionTheme()
        {
            IconBubble.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FFF6E7"));

            txtIcon.Text = "💳";
        }

        public void SetUserTheme()
        {
            IconBubble.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#F3EEFF"));

            txtIcon.Text = "👤";
        }

        public void SetLoyaltyTheme()
        {
            IconBubble.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FFF9E8"));

            txtIcon.Text = "⭐";
        }
    }
}