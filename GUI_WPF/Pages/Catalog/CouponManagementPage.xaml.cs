using BUS;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI_WPF.Pages.Catalog
{
    public partial class CouponManagementPage : Page
    {
        private readonly BUS_Coupon _bus = new BUS_Coupon();

        private List<ET_Coupons> _allCoupons = new List<ET_Coupons>();

        private string _searchKeyword = string.Empty;

        private string _statusFilter = "All";

        public CouponManagementPage()
        {
            InitializeComponent();

            LoadStatusFilterItems();

            LoadData();
        }

        // =============================================
        // INIT
        // =============================================
        private void LoadStatusFilterItems()
        {
            cbStatusFilter.Items.Add("All");
            cbStatusFilter.Items.Add("Active");
            cbStatusFilter.Items.Add("Upcoming");
            cbStatusFilter.Items.Add("Expired");
            cbStatusFilter.Items.Add("Used Up");
            cbStatusFilter.Items.Add("Inactive");
            cbStatusFilter.SelectedIndex = 0;
        }

        // =============================================
        // LOAD
        // =============================================
        public void LoadData()
        {
            _allCoupons = _bus.GetAll();

            UpdateStats();
            ApplyFilter();
        }

        private void UpdateStats()
        {
            int total = _allCoupons.Count;

            int active = _allCoupons.Count(c => c.StatusText == "Active");

            int expired = _allCoupons.Count(c => c.StatusText == "Expired");

            int inactiveOrUsedUp = _allCoupons.Count(c =>
                c.StatusText == "Used Up" || c.StatusText == "Inactive");

            txtTotalCount.Text = total.ToString();
            txtActiveCount.Text = active.ToString();
            txtExpiredCount.Text = expired.ToString();
            txtInactiveCount.Text = inactiveOrUsedUp.ToString();
        }

        private void ApplyFilter()
        {
            IEnumerable<ET_Coupons> filtered = _allCoupons;

            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                string kw = _searchKeyword.ToLower();
                filtered = filtered.Where(c =>
                    (c.CouponCode ?? "").ToLower().Contains(kw));
            }

            if (_statusFilter != "All")
            {
                filtered = filtered.Where(c => c.StatusText == _statusFilter);
            }

            var list = filtered.ToList();

            dgCoupons.ItemsSource = list;

            // Toggle empty state
            bool isEmpty = list.Count == 0;
            EmptyState.Visibility = isEmpty
                ? Visibility.Visible
                : Visibility.Collapsed;
            dgCoupons.Visibility = isEmpty
                ? Visibility.Collapsed
                : Visibility.Visible;

            // Adjust empty state copy depending on whether the source is truly empty
            // or just filtered to nothing
            if (isEmpty)
            {
                if (_allCoupons.Count == 0)
                {
                    EmptyState.Title = "No Coupons Yet";
                    EmptyState.Description = "Create your first discount coupon to offer to guests during booking.";
                    EmptyState.ButtonText = "Create First Coupon";
                }
                else
                {
                    EmptyState.Title = "No Results Found";
                    EmptyState.Description = "No coupons match the current search or filter. Try adjusting them.";
                    EmptyState.ButtonText = "";
                }
            }
        }

        // =============================================
        // TOOLBAR EVENTS
        // =============================================
        private void sb_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchKeyword = sb.SearchText?.Trim() ?? string.Empty;
            ApplyFilter();
        }

        private void cbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _statusFilter = cbStatusFilter.SelectedItem?.ToString() ?? "All";
            ApplyFilter();
        }

        // =============================================
        // CRUD ACTIONS
        // =============================================
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            OpenEditDialog(null);
        }

        private void EmptyState_ActionClicked(object sender, EventArgs e)
        {
            OpenEditDialog(null);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var coupon = btn?.Tag as ET_Coupons;

            if (coupon == null)
                return;

            OpenEditDialog(coupon);
        }

        private void dgCoupons_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var coupon = dgCoupons.SelectedItem as ET_Coupons;
            if (coupon == null)
                return;

            OpenEditDialog(coupon);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var coupon = btn?.Tag as ET_Coupons;

            if (coupon == null)
                return;

            // Prevent delete if already used
            if (coupon.CurrentUsageCount > 0)
            {
                MessageBox.Show(
                    $"Coupon '{coupon.CouponCode}' has already been used {coupon.CurrentUsageCount} time(s) and cannot be deleted.\n\n" +
                    "Consider deactivating it instead.",
                    "Cannot Delete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete coupon '{coupon.CouponCode}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            bool ok = _bus.Delete(coupon.ID);

            if (!ok)
            {
                MessageBox.Show(
                    "Unable to delete this coupon. It may be referenced by existing bookings.",
                    "Delete Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            LoadData();
        }

        // =============================================
        // DIALOG
        // =============================================
        private void OpenEditDialog(ET_Coupons coupon)
        {
            var dlg = new CouponEditDialog(coupon);
            dlg.Owner = Window.GetWindow(this);

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }
    }
}