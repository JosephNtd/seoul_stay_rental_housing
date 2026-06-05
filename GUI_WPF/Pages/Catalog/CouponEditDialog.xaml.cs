using BUS;
using ET;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace GUI_WPF.Pages.Catalog
{
    public partial class CouponEditDialog : Window
    {
        private readonly BUS_Coupon _bus = new BUS_Coupon();

        private readonly ET_Coupons _entity;

        private readonly bool _isEditMode;

        // =============================================
        // CONSTRUCTOR
        // =============================================
        public CouponEditDialog(ET_Coupons coupon)
        {
            InitializeComponent();

            _entity = coupon;
            _isEditMode = coupon != null;

            ApplyMode();

            // Allow dragging the window since WindowStyle="None"
            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            };
        }

        // =============================================
        // MODE / POPULATE
        // =============================================
        private void ApplyMode()
        {
            if (_isEditMode)
            {
                txtTitle.Text = "Edit Coupon";
                txtSubtitle.Text = $"Update details for '{_entity.CouponCode}'.";

                PopulateFields();

                // Show usage info panel
                UsageInfoPanel.Visibility = Visibility.Visible;

                string usage = _entity.MaxUsageCount.HasValue
                    ? $"Currently used: {_entity.CurrentUsageCount} / {_entity.MaxUsageCount.Value} time(s)."
                    : $"Currently used: {_entity.CurrentUsageCount} time(s) (unlimited cap).";

                txtUsageInfo.Text = usage;
            }
            else
            {
                txtTitle.Text = "New Coupon";
                txtSubtitle.Text = "Create a discount coupon for guest bookings.";

                // Default values for new coupon
                dpStart.SelectedDate = DateTime.Today;
                chkActive.IsChecked = true;
            }
        }

        private void PopulateFields()
        {
            txtCode.Text = _entity.CouponCode;
            txtDiscountPercent.Text = _entity.DiscountPercent.ToString("0.##");
            txtMaxDiscount.Text = _entity.MaximumDiscountAmount.ToString("0");

            dpStart.SelectedDate = _entity.StartedDate;
            dpExpiration.SelectedDate = _entity.ExpirationDate;

            txtMaxUsage.Text = _entity.MaxUsageCount.HasValue
                ? _entity.MaxUsageCount.Value.ToString()
                : string.Empty;

            chkActive.IsChecked = _entity.IsActive;
        }

        // =============================================
        // VALIDATION
        // =============================================
        private bool Validate(out string error)
        {
            error = string.Empty;

            // CODE
            string code = txtCode.Text?.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                error = "Coupon code is required.";
                txtCode.Focus();
                return false;
            }

            if (!Regex.IsMatch(code, @"^[A-Za-z0-9_\-]+$"))
            {
                error = "Coupon code can only contain letters, numbers, underscore (_) and dash (-).";
                txtCode.Focus();
                return false;
            }

            long idToIgnore = _isEditMode ? _entity.ID : 0;

            if (_bus.IsCodeExists(code, idToIgnore))
            {
                error = $"Coupon code '{code}' already exists.";
                txtCode.Focus();
                return false;
            }

            // DISCOUNT %
            if (!decimal.TryParse(txtDiscountPercent.Text, out decimal percent))
            {
                error = "Discount percent must be a valid number.";
                txtDiscountPercent.Focus();
                return false;
            }

            if (percent <= 0 || percent > 100)
            {
                error = "Discount percent must be between 0.01 and 100.";
                txtDiscountPercent.Focus();
                return false;
            }

            // MAX DISCOUNT
            if (!decimal.TryParse(txtMaxDiscount.Text, out decimal maxDiscount))
            {
                error = "Maximum discount must be a valid number.";
                txtMaxDiscount.Focus();
                return false;
            }

            if (maxDiscount < 0)
            {
                error = "Maximum discount cannot be negative.";
                txtMaxDiscount.Focus();
                return false;
            }

            // START DATE
            if (!dpStart.SelectedDate.HasValue)
            {
                error = "Start date is required.";
                dpStart.Focus();
                return false;
            }

            // EXPIRATION DATE (optional)
            if (dpExpiration.SelectedDate.HasValue)
            {
                if (dpExpiration.SelectedDate.Value.Date <= dpStart.SelectedDate.Value.Date)
                {
                    error = "Expiration date must be after the start date.";
                    dpExpiration.Focus();
                    return false;
                }
            }

            // MAX USAGE (optional)
            string maxUsageText = txtMaxUsage.Text?.Trim();
            int? maxUsage = null;

            if (!string.IsNullOrWhiteSpace(maxUsageText))
            {
                if (!int.TryParse(maxUsageText, out int parsed))
                {
                    error = "Maximum usage count must be a whole number.";
                    txtMaxUsage.Focus();
                    return false;
                }

                if (parsed <= 0)
                {
                    error = "Maximum usage count must be greater than zero.";
                    txtMaxUsage.Focus();
                    return false;
                }

                // For edit: must not be lower than current usage
                if (_isEditMode && parsed < _entity.CurrentUsageCount)
                {
                    error = $"Maximum usage cannot be lower than current usage ({_entity.CurrentUsageCount}).";
                    txtMaxUsage.Focus();
                    return false;
                }

                maxUsage = parsed;
            }

            return true;
        }

        // =============================================
        // BUILD ENTITY
        // =============================================
        private ET_Coupons BuildEntity()
        {
            int? maxUsage = null;
            string maxUsageText = txtMaxUsage.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(maxUsageText) && int.TryParse(maxUsageText, out int parsedMax))
                maxUsage = parsedMax;

            ET_Coupons et = _isEditMode
                ? _entity
                : new ET_Coupons { GUID = Guid.NewGuid(), CurrentUsageCount = 0 };

            et.CouponCode = txtCode.Text.Trim();
            et.DiscountPercent = decimal.Parse(txtDiscountPercent.Text);
            et.MaximumDiscountAmount = decimal.Parse(txtMaxDiscount.Text);
            et.StartedDate = dpStart.SelectedDate.Value;
            et.ExpirationDate = dpExpiration.SelectedDate;
            et.MaxUsageCount = maxUsage;
            et.IsActive = chkActive.IsChecked == true;

            return et;
        }

        // =============================================
        // ACTIONS
        // =============================================
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate(out string error))
            {
                MessageBox.Show(error, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ET_Coupons et = BuildEntity();

            bool ok = _isEditMode
                ? _bus.Update(et)
                : _bus.Insert(et);

            if (!ok)
            {
                MessageBox.Show(
                    _isEditMode
                        ? "Unable to update the coupon. Please try again."
                        : "Unable to create the coupon. Please try again.",
                    "Save Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBox.Show(
                _isEditMode
                    ? "Coupon updated successfully."
                    : "Coupon created successfully.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // =============================================
        // INPUT FILTERS
        // =============================================
        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow digits and a single decimal separator
            var tb = sender as System.Windows.Controls.TextBox;
            string current = tb?.Text ?? string.Empty;

            // Reject non-numeric
            if (!Regex.IsMatch(e.Text, @"^[0-9\.,]$"))
            {
                e.Handled = true;
                return;
            }

            // Reject second decimal separator
            if ((e.Text == "." || e.Text == ",") && (current.Contains(".") || current.Contains(",")))
            {
                e.Handled = true;
            }
        }

        private void IntegerOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[0-9]$"))
                e.Handled = true;
        }
    }
}