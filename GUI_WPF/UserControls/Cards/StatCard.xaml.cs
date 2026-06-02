using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Cards
{
    public partial class StatCard : UserControl
    {
        public StatCard()
        {
            InitializeComponent();
        }

        // TITLE
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(StatCard),
                new PropertyMetadata("Title"));

        // VALUE
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(string),
                typeof(StatCard),
                new PropertyMetadata("0"));

        // ICON
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(string),
                typeof(StatCard),
                new PropertyMetadata("📊"));

        // TREND
        public string Trend
        {
            get { return (string)GetValue(TrendProperty); }
            set { SetValue(TrendProperty, value); }
        }

        public static readonly DependencyProperty TrendProperty =
            DependencyProperty.Register(
                nameof(Trend),
                typeof(string),
                typeof(StatCard),
                new PropertyMetadata("+0%"));

        // DESCRIPTION
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(
                nameof(Description),
                typeof(string),
                typeof(StatCard),
                new PropertyMetadata("vs last month"));

        // TREND BACKGROUND
        public Brush TrendBackground
        {
            get { return (Brush)GetValue(TrendBackgroundProperty); }
            set { SetValue(TrendBackgroundProperty, value); }
        }

        public static readonly DependencyProperty TrendBackgroundProperty =
            DependencyProperty.Register(
                nameof(TrendBackground),
                typeof(Brush),
                typeof(StatCard),
                new PropertyMetadata(
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#EAF8F0"))));

        // TREND FOREGROUND
        public Brush TrendForeground
        {
            get { return (Brush)GetValue(TrendForegroundProperty); }
            set { SetValue(TrendForegroundProperty, value); }
        }

        public static readonly DependencyProperty TrendForegroundProperty =
            DependencyProperty.Register(
                nameof(TrendForeground),
                typeof(Brush),
                typeof(StatCard),
                new PropertyMetadata(
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#2E7D5A"))));
    }
}
