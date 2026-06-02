using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.UserControls.Common
{
    public partial class EmptyState : UserControl
    {
        public EmptyState()
        {
            InitializeComponent();

            Loaded += EmptyState_Loaded;
        }

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
                typeof(EmptyState),
                new PropertyMetadata("📦"));

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
                typeof(EmptyState),
                new PropertyMetadata("Nothing Here Yet"));

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
                typeof(EmptyState),
                new PropertyMetadata(
                    "There is currently no data available in this section."));

        // BUTTON TEXT
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(
                nameof(ButtonText),
                typeof(string),
                typeof(EmptyState),
                new PropertyMetadata(string.Empty));

        // CLICK EVENT
        public event EventHandler ActionClicked;

        private void EmptyState_Loaded(object sender, RoutedEventArgs e)
        {
            btnAction.Visibility =
                string.IsNullOrWhiteSpace(ButtonText)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            ActionClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
