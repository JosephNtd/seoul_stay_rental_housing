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
using System.Windows.Shapes;
using GUI_WPF.Pages.Catalog;
using GUI_WPF.Pages.Dashboard;
using GUI_WPF.Pages.Users;

namespace GUI_WPF.Views
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private ET_Users _currentUser;
        public AdminWindow(ET_Users user)
        {
            InitializeComponent();
            _currentUser = user;
            Sidebar.SetUserInfo(
                _currentUser.FullName,
                _currentUser.IsAdmin ? "Administrator" : "Host"
            );

            Sidebar.DashboardClicked += btnDashboard_Click;
            Sidebar.LocationCatalogClicked += btnLocationCatalog_Click;
            Sidebar.PropertyTypesClicked += btnPropertyTypes_Click;
            Sidebar.ServicesClicked += btnServices_Click;
            Sidebar.CouponsClicked += btnCoupons_Click;
            Sidebar.UsersClicked += btnUsers_Click;
            Sidebar.ListingsClicked += btnListings_Click;
            Sidebar.BookingsClicked += btnBookings_Click;
            Sidebar.ReportsClicked += btnReports_Click;

            MainFrame.Navigate(new Pages.Dashboard.DashboardPage());
        }
        // LOAD USER INFO
        
        private void Navigate(Page page)
        {
            MainFrame.Navigate(page);
        }
        // DASHBOARD 
        private void btnDashboard_Click(object sender, RoutedEventArgs e) 
        { 
            Navigate(new DashboardPage()); 
        } 
        // LOCATION WORKSPACE
        private void btnLocationCatalog_Click(object sender, RoutedEventArgs e) 
        { 
            Navigate(new LocationWorkspacePage()); 
        } 
        // PROPERTY TYPES
        private void btnPropertyTypes_Click(object sender, RoutedEventArgs e) 
        {
            Navigate(new PropertyTypesPage());
        } 
        // SERVICES 
        private void btnServices_Click(object sender, RoutedEventArgs e) 
        {
            Navigate(new ServicesWorkspacePage());
        } 
        // TRANSACTIONS
        private void btnCoupons_Click(object sender, RoutedEventArgs e) 
        {
            Navigate(new CouponManagementPage());
        } 
        // USERS
        private void btnUsers_Click(object sender, RoutedEventArgs e) 
        {
            Navigate(new UsersPage());
        }
        // LISTINGS
        private void btnListings_Click(object sender, RoutedEventArgs e) 
        {
            Navigate(new Pages.Listings.ListingsPage());
        } 
        // BOOKINGS
        private void btnBookings_Click(object sender, RoutedEventArgs e) 
        {
            Navigate(new Pages.Bookings.BookingsPage(_currentUser));
        } 
        // REPORTS
        private void btnReports_Click(object sender, RoutedEventArgs e) 
        {
            Navigate(new Pages.Reports.ReportsPage());
        }
    }
}
