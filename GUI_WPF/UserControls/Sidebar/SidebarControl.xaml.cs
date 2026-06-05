using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI_WPF.UserControls.Sidebar
{
    /// <summary>
    /// Interaction logic for SidebarControl.xaml
    /// </summary>
    public partial class SidebarControl : UserControl
    {
        public event RoutedEventHandler DashboardClicked;
        public event RoutedEventHandler LocationCatalogClicked;
        public event RoutedEventHandler PropertyTypesClicked;
        public event RoutedEventHandler ServicesClicked;
        public event RoutedEventHandler CouponsClicked;
        public event RoutedEventHandler UsersClicked;
        public event RoutedEventHandler ListingsClicked;
        public event RoutedEventHandler BookingsClicked;
        public event RoutedEventHandler ReportsClicked;

        private bool _isCollapsed;
        public bool IsCollapsed => _isCollapsed;

        public event EventHandler<bool> SidebarStateChanged;
        public SidebarControl()
        {
            InitializeComponent();

        }
        private void BtnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            _isCollapsed = !_isCollapsed;

            UpdateSidebarState();

            SidebarStateChanged?.Invoke(this, _isCollapsed);
        }

        private void UpdateSidebarState()
        {
            if (_isCollapsed)
            {
                Width = 80;
                btnToggleSidebar.Content = "✕";
                // Logo
                txtLogo.Visibility = Visibility.Collapsed;
                txtSubLogo.Visibility = Visibility.Collapsed;

                // Profile
                txtAdminName.Visibility = Visibility.Collapsed;
                txtAdminRole.Visibility = Visibility.Collapsed;

                // Menu text
                txtDashboard.Visibility = Visibility.Collapsed;
                txtUsers.Visibility = Visibility.Collapsed;
                txtListings.Visibility = Visibility.Collapsed;
                txtBookings.Visibility = Visibility.Collapsed;
                txtReports.Visibility = Visibility.Collapsed;

                // Catalog
                expCatalog.Visibility = Visibility.Collapsed;

                // Tooltips
                btnDashboard.ToolTip = "Dashboard";
                btnUsers.ToolTip = "Users";
                btnListings.ToolTip = "Listings";
                btnBookings.ToolTip = "Bookings";
                btnReports.ToolTip = "Reports";
            }
            else
            {
                Width = 280;

                txtLogo.Visibility = Visibility.Visible;
                txtSubLogo.Visibility = Visibility.Visible;

                txtAdminName.Visibility = Visibility.Visible;
                txtAdminRole.Visibility = Visibility.Visible;

                txtDashboard.Visibility = Visibility.Visible;
                txtUsers.Visibility = Visibility.Visible;
                txtListings.Visibility = Visibility.Visible;
                txtBookings.Visibility = Visibility.Visible;
                txtReports.Visibility = Visibility.Visible;

                expCatalog.Visibility = Visibility.Visible;

                btnDashboard.ToolTip = null;
                btnUsers.ToolTip = null;
                btnListings.ToolTip = null;
                btnBookings.ToolTip = null;
                btnReports.ToolTip = null;
            }
        }
        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardClicked?.Invoke(this, e);
        }

        private void btnLocationCatalog_Click(object sender, RoutedEventArgs e)
        {
            LocationCatalogClicked?.Invoke(this, e);
        }

        private void btnPropertyTypes_Click(object sender, RoutedEventArgs e)
        {
            PropertyTypesClicked?.Invoke(this, e);
        }

        private void btnServices_Click(object sender, RoutedEventArgs e)
        {
            ServicesClicked?.Invoke(this, e);
        }

        private void btnCoupons_Click(object sender, RoutedEventArgs e)
        {
            CouponsClicked?.Invoke(this, e);
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            UsersClicked?.Invoke(this, e);
        }

        private void btnListings_Click(object sender, RoutedEventArgs e)
        {
            ListingsClicked?.Invoke(this, e);
        }

        private void btnBookings_Click(object sender, RoutedEventArgs e)
        {
            BookingsClicked?.Invoke(this, e);
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsClicked?.Invoke(this, e);
        }

        public void SetUserInfo(string name, string role)
        {
            txtAdminName.Text = name;
            txtAdminRole.Text = role;
        }
    }
}
