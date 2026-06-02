using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.UserControls.Drawers
{
    public partial class PropertyTypeDrawer : UserControl
    {
        public PropertyTypeDrawer()
        {
            InitializeComponent();
        }

         
        // EVENTS
         

        public event RoutedEventHandler SaveClicked;

        public event RoutedEventHandler DeleteClicked;

        public event RoutedEventHandler ChooseIconClicked;

        private void btnSave_Click(
            object sender,
            RoutedEventArgs e)
        {
            SaveClicked?.Invoke(this, e);
        }

        private void btnDelete_Click(
            object sender,
            RoutedEventArgs e)
        {
            DeleteClicked?.Invoke(this, e);
        }

        private void btnChooseIcon_Click(
            object sender,
            RoutedEventArgs e)
        {
            ChooseIconClicked?.Invoke(this, e);
        }
    }
}