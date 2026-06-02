using DTO;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI_WPF.UserControls.Cards
{
    public partial class AttractionRow : UserControl
    {
        public AttractionRow()
        {
            InitializeComponent();
            Cursor = Cursors.Hand;
        }

        // DATA
        public DTO_Attraction_Distance Attraction { get; private set; }

        // EVENTS
        public event EventHandler RemoveClicked;

        // LOAD
        public void LoadAttraction(DTO_Attraction_Distance attraction)
        {
            if (attraction == null) return;

            Attraction = attraction;

            // NAME
            txtAttractionName.Text = string.IsNullOrWhiteSpace(attraction.Attraction)
                ? "Unknown Attraction" : attraction.Attraction;

            // ADDRESS
            txtAddress.Text = "Nearby attraction location";

            // AREA
            txtArea.Text = string.IsNullOrWhiteSpace(attraction.Area)
                ? "Unknown Area" : attraction.Area;

            // EDITABLE FIELDS
            txtDistance.Text = attraction.Distance?.ToString("0.#") ?? "";
            txtWalk.Text = attraction.OnFoot?.ToString() ?? "";
            txtDrive.Text = attraction.ByCar?.ToString() ?? "";
        }

        // Cập nhật dữ liệu từ UI về DTO trước khi Save
        public void UpdateAttractionData()
        {
            if (Attraction == null) return;

            // Distance là decimal?
            if (decimal.TryParse(txtDistance.Text, out decimal distance))
                Attraction.Distance = distance;
            else
                Attraction.Distance = null;

            // OnFoot và ByCar là int?
            if (int.TryParse(txtWalk.Text, out int walk))
                Attraction.OnFoot = walk;
            else
                Attraction.OnFoot = null;

            if (int.TryParse(txtDrive.Text, out int drive))
                Attraction.ByCar = drive;
            else
                Attraction.ByCar = null;
        }

        // Chỉ cho phép nhập số và dấu chấm (cho Distance)
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9.]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtDistance_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txtWalk_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txtDrive_TextChanged(object sender, TextChangedEventArgs e) { }

        // REMOVE
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}