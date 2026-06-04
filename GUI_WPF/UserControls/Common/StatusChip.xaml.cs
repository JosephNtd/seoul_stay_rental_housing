using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Common
{
    public partial class StatusChip : UserControl
    {
        public StatusChip()
        {
            InitializeComponent();

            Loaded += StatusChip_Loaded;
        }

        // TEXT
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(StatusChip),
                new PropertyMetadata("Status"));

        // STATUS TYPE
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(
                nameof(Status),
                typeof(string),
                typeof(StatusChip),
                new PropertyMetadata("default"));

        // FOREGROUND
        public Brush ForegroundColor
        {
            get { return (Brush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(
                nameof(ForegroundColor),
                typeof(Brush),
                typeof(StatusChip),
                new PropertyMetadata(
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#D89C9D"))));

        private void StatusChip_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyStatusTheme();
        }

        private void ApplyStatusTheme()
        {
            switch (Status.ToLower())
            {
                // SUCCESS
                case "confirmed":
                case "completed":
                case "active":
                case "paid":

                    ChipBorder.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#EAF8F0"));

                    ChipBorder.BorderBrush =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#CFEEDD"));

                    StatusDot.Fill =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#2E7D5A"));

                    ForegroundColor =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#2E7D5A"));

                    break;

                // WARNING
                case "pending":
                case "processing":
                case "upcoming":

                    ChipBorder.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FFF8E8"));

                    ChipBorder.BorderBrush =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#F5E2A8"));

                    StatusDot.Fill =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#B98500"));

                    ForegroundColor =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#B98500"));

                    break;

                // ERROR
                case "cancelled":
                case "rejected":
                case "failed":
                case "refund":
                case "inactive":
                case "expired":
                case "used up":

                    ChipBorder.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FDECEC"));

                    ChipBorder.BorderBrush =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#F8CACA"));

                    StatusDot.Fill =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D32F2F"));

                    ForegroundColor =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D32F2F"));

                    break;

                // INFO
                case "draft":
                case "review":
                case "checking":

                    ChipBorder.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#EEF4FF"));

                    ChipBorder.BorderBrush =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D7E5FF"));

                    StatusDot.Fill =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#4F7BDB"));

                    ForegroundColor =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#4F7BDB"));

                    break;

                // DEFAULT
                default:

                    ChipBorder.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FFF0F0"));

                    ChipBorder.BorderBrush =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FFE0E0"));

                    StatusDot.Fill =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D89C9D"));

                    ForegroundColor =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D89C9D"));

                    break;
            }
        }
    }
}