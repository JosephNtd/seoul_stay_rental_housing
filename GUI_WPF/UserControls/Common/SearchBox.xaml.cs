using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.UserControls.Common
{
    public partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            InitializeComponent();

            Loaded += SearchBox_Loaded;

            txtSearch.TextChanged += TxtSearch_TextChanged;

            txtSearch.GotFocus += TxtSearch_GotFocus;

            txtSearch.LostFocus += TxtSearch_LostFocus;
        }

        // ROUTED EVENT
        public static readonly RoutedEvent TextChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(TextChanged),
                RoutingStrategy.Bubble,
                typeof(TextChangedEventHandler),
                typeof(SearchBox));

        public event TextChangedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        // PLACEHOLDER
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(SearchBox),
                new PropertyMetadata("Search..."));

        // SEARCH TEXT
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                nameof(SearchText),
                typeof(string),
                typeof(SearchBox),
                new PropertyMetadata(string.Empty));

        private void SearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholder();
            UpdateClearButton();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchText = txtSearch.Text;

            UpdatePlaceholder();

            UpdateClearButton();

            // RAISE EVENT
            RaiseEvent(new TextChangedEventArgs(TextChangedEvent, UndoAction.None));
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBorder.BorderBrush =
                (System.Windows.Media.Brush)
                FindResource("PrimaryBrush");

            SearchBorder.Background =
                System.Windows.Media.Brushes.White;
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchBorder.BorderBrush =
                (System.Windows.Media.Brush)
                FindResource("BorderBrush");
        }

        private void UpdatePlaceholder()
        {
            txtPlaceholder.Visibility =
                string.IsNullOrWhiteSpace(txtSearch.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void UpdateClearButton()
        {
            btnClear.Visibility =
                string.IsNullOrWhiteSpace(txtSearch.Text)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
        }
    }
}