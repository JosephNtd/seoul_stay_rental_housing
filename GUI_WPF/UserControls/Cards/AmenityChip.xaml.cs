using DTO;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI_WPF.UserControls.Cards
{
    public partial class AmenityChip : UserControl
    {
        public AmenityChip()
        {
            InitializeComponent();

            UpdateVisualState();
        }

         
        // DATA
         

        public DTO_Amenities Amenity
        {
            get;
            private set;
        }

         
        // PROPERTIES
         

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                _isSelected = value;

                UpdateVisualState();
            }
        }

         
        // EVENTS
         

        public event EventHandler SelectionChanged;

         
        // LOAD
         

        public void LoadAmenity(
    DTO_Amenities amenity)
        {
            if (amenity == null)
                return;

            Amenity = amenity;

             
            // NAME
             

            txtAmenityName.Text =
                string.IsNullOrWhiteSpace(
                    amenity.Name)
                ? "Amenity"
                : amenity.Name;

             
            // DESCRIPTION
             

            txtAmenityDescription.Text =
                string.IsNullOrWhiteSpace(
                    amenity.Description)
                ? "Property amenity"
                : amenity.Description;

             
            // SELECTED STATE
             

            IsSelected =
                amenity.IsSelected;

             
            // ICON
             

            try
            {
                if (!string.IsNullOrWhiteSpace(
                    amenity.IconPath))
                {
                    string fullPath =
                        amenity.IconPath;

                    // Nếu chỉ là tên file
                    if (!System.IO.Path.IsPathRooted(fullPath))
                    {
                        fullPath =
                            System.IO.Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                "Images",
                                amenity.IconPath);
                    }

                    if (System.IO.File.Exists(fullPath))
                    {
                        imgIcon.Source =
                            new BitmapImage(
                                new Uri(
                                    fullPath,
                                    UriKind.Absolute));
                    }
                }
            }
            catch
            {

            }

             
            // APPLY UI STATE
             

            UpdateVisualState();
        }

         
        // VISUAL STATE
         

        private void UpdateVisualState()
        {
            if (IsSelected)
            {
                RootBorder.Background =
                    new SolidColorBrush(
                        Color.FromRgb(255, 244, 240));

                RootBorder.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(199, 128, 107));

                SelectionBadge.Background =
                    new SolidColorBrush(
                        Color.FromRgb(123, 84, 85));

                txtCheck.Visibility =
                    Visibility.Visible;

                IconContainer.Background =
                    new SolidColorBrush(
                        Color.FromRgb(246, 223, 214));
            }
            else
            {
                RootBorder.Background =
                    new SolidColorBrush(
                        Color.FromRgb(255, 248, 244));

                RootBorder.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(232, 216, 210));

                SelectionBadge.Background =
                    new SolidColorBrush(
                        Color.FromRgb(242, 230, 225));

                txtCheck.Visibility =
                    Visibility.Collapsed;

                IconContainer.Background =
                    new SolidColorBrush(
                        Color.FromRgb(246, 236, 231));
            }
        }

         
        // CLICK
         

        private void RootBorder_MouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            IsSelected =
                !IsSelected;

            if (Amenity != null)
            {
                Amenity.IsSelected =
                    IsSelected;
            }

            SelectionChanged?.Invoke(
                this,
                EventArgs.Empty);
        }
    }
}