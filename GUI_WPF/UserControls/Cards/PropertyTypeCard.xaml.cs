using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.UserControls.Cards
{
    public partial class PropertyTypeCard : UserControl
    {
        public PropertyTypeCard()
        {
            InitializeComponent();
        }

         
        // DEPENDENCY PROPERTIES
         

        public string Title
        {
            get => txtName.Text;
            set => txtName.Text = value;
        }

        public string Description
        {
            get => txtDescription.Text;
            set => txtDescription.Text = value;
        }

        public string ListingCount
        {
            get => txtCount.Text;
            set => txtCount.Text = value;
        }

        public string Emoji
        {
            get => txtFallbackEmoji.Text;
            set => txtFallbackEmoji.Text = value;
        }

         
        // EVENTS
         

        public event RoutedEventHandler EditClicked;

        public event RoutedEventHandler PreviewClicked;

        private void btnEdit_Click(
            object sender,
            RoutedEventArgs e)
        {
            EditClicked?.Invoke(this, e);
        }

        private void btnPreview_Click(
            object sender,
            RoutedEventArgs e)
        {
            PreviewClicked?.Invoke(this, e);
        }
    }
}
