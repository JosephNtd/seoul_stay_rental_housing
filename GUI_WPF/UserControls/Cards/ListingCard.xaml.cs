using DTO;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GUI_WPF.UserControls.Cards
{
    public partial class ListingCard : UserControl
    {
        public ListingCard()
        {
            InitializeComponent();

            Cursor = Cursors.Hand;
        }

        // DATA
        public DTO_ItemCard Listing { get; private set; }

        // EVENTS
        public event EventHandler OpenClicked;
        public event EventHandler EditClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler PricingClicked;
        public event EventHandler AvailabilityClicked;
        public event EventHandler CardClicked;

        private void RootBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => CardClicked?.Invoke(this, EventArgs.Empty);


        // LOAD DATA
        public void LoadListing(DTO_ItemCard item)
        {
            if (item == null)
                return;

            Listing = item;

            // BASIC INFO             
            txtTitle.Text = item.Title;
            txtItemType.Text = item.Type;
            txtArea.Text = item.AreaName;
            txtAddress.Text = item.ApproximateAddress;

            // STATS
            txtCapacity.Text = item.Capacity.ToString();
            txtBedrooms.Text = item.NumberOfBedrooms.ToString();
            txtBeds.Text = item.NumberOfBeds.ToString();
            txtBathrooms.Text = item.NumberOfBathrooms.ToString();


            // HOST
            txtHost.Text = string.IsNullOrWhiteSpace(item.HostName) ? "Unknown Host" : item.HostName;


            // PRICE             
            txtPrice.Text = item.MinPrice != null ? $"${item.MinPrice:0}" : "$0";


            // STATUS             
            txtStatus.Text = string.IsNullOrWhiteSpace(item.Status) ? "Draft" : item.Status;

            switch (txtStatus.Text.ToLower())
            {
                case "active":
                    StatusBadge.Background = System.Windows.Media.Brushes.Honeydew;
                    txtStatus.Foreground = System.Windows.Media.Brushes.ForestGreen;
                    break;

                case "occupied":
                    StatusBadge.Background = System.Windows.Media.Brushes.LemonChiffon;
                    txtStatus.Foreground = System.Windows.Media.Brushes.DarkOrange;
                    break;

                case "inactive":
                    StatusBadge.Background = System.Windows.Media.Brushes.MistyRose;
                    txtStatus.Foreground = System.Windows.Media.Brushes.Firebrick;
                    break;

                default:
                    StatusBadge.Background = System.Windows.Media.Brushes.Gainsboro;
                    txtStatus.Foreground = System.Windows.Media.Brushes.DimGray;
                    break;
            }


            // IMAGE            
            try
            {
                if (!string.IsNullOrWhiteSpace(item.FullThumbnailPath))
                {
                    imgThumbnail.Source = new BitmapImage(new Uri(item.FullThumbnailPath, UriKind.Absolute));
                }
            }
            catch
            {

            }
            this.MinHeight = 520;
        }

        // OPEN
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenClicked?.Invoke(this, EventArgs.Empty);
        }


        // MENU
        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            btnMore.ContextMenu.IsOpen = true;
        }

        private void miEdit_Click(object sender, RoutedEventArgs e)
        {
            EditClicked?.Invoke(this, EventArgs.Empty);
        }

        private void miDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteClicked?.Invoke(this, EventArgs.Empty);
        }

        private void miManagePricing_Click(object sender, RoutedEventArgs e)
        {
            PricingClicked?.Invoke(this, EventArgs.Empty);
        }

        private void miManageAvailability_Click(object sender, RoutedEventArgs e)
        {
            AvailabilityClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}