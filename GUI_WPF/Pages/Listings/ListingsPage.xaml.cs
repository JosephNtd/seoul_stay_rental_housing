using BUS;
using DTO;
using ET;
using GUI_WPF.Pages.Listings;
using GUI_WPF.UserControls.Cards;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.Pages.Listings
{
    public partial class ListingsPage : Page
    {
        public ListingsPage()
        {
            InitializeComponent();
            InitializeFilters();
            LoadListings();
            WireDrawerEvents();
        }


        // BUS


        private readonly BUS_Items _busItems = new BUS_Items();
        private readonly BUS_ItemType _busItemType = new BUS_ItemType();
        private readonly BUS_Amenity _busAmenity = new BUS_Amenity();
        private readonly BUS_Attraction _busAttraction = new BUS_Attraction();
        private readonly BUS_ItemPictures _busPictures = new BUS_ItemPictures();

        // DATA
        private List<DTO_ItemCard> _allListings = new List<DTO_ItemCard>();
        private DTO_ItemCard _selectedListing;

        // INIT
        private void InitializeFilters()
        {
            cbStatusFilter.Items.Add("All");
            cbStatusFilter.Items.Add("Active");
            cbStatusFilter.Items.Add("Draft");

            cbStatusFilter.SelectedIndex = 0;

            cbSort.Items.Add("Newest");
            cbSort.Items.Add("A-Z");
            cbSort.Items.Add("Price");

            cbSort.SelectedIndex = 0;
        }

        private void WireDrawerEvents()
        {
            listingEditorDrawer.CloseClicked += ListingEditorDrawer_CloseClicked;
            listingEditorDrawer.SaveClicked += ListingEditorDrawer_SaveClicked;
            listingEditorDrawer.DeleteClicked += ListingEditorDrawer_DeleteClicked;
        }


        // LOAD


        private void LoadListings()
        {
            try
            {
                _allListings = _busItems.GetItemCards();

                RenderListings(_allListings);

                UpdateStats();

                LoadTypeFilter();
            }
            catch
            {
                MessageBox.Show("Cannot load listings.");
            }
        }

        private void LoadTypeFilter()
        {
            cbTypeFilter.Items.Clear();

            cbTypeFilter.Items.Add("All");

            var types = _allListings.Select(x => x.Type).Distinct().OrderBy(x => x);

            foreach (var type in types)
            {
                cbTypeFilter.Items.Add(type);
            }

            cbTypeFilter.SelectedIndex = 0;
        }


        // RENDER


        private void RenderListings(List<DTO_ItemCard> data)
        {
            wpListings.Children.Clear();

            if (data == null)
                return;

            foreach (var item in data)
            {
                ListingCard card = new ListingCard();

                card.Width = 330;
                //card.Height = 460;
                card.Margin = new Thickness(0, 0, 24, 24);

                card.LoadListing(item);
                // OPEN
                card.OpenClicked += (s, e) =>
                    {
                        OpenListing(item);
                    };

                // EDIT
                card.EditClicked += (s, e) =>
                    {
                        EditListing(item);
                    };

                // DELETE
                card.DeleteClicked += (s, e) =>
                    {
                        DeleteListing(item);
                    };

                // PRICING
                card.PricingClicked += (s, e) =>
                    {
                        OpenCalendar(item);
                    };

                // AVAILABILITY
                card.AvailabilityClicked += (s, e) =>
                    {
                        OpenCalendar(item);
                    };

                wpListings.Children.Add(card);
            }
            
        }


        // STATS
        private void UpdateStats()
        {
            txtTotalListings.Text = _allListings.Count.ToString();
            txtActiveListings.Text = _allListings.Count(x => x.Status == "Active").ToString();
            txtDraftListings.Text = _allListings.Count(x => x.Status == "Draft").ToString();
        }


        // SEARCH
        private void ApplyFilters()
        {
            var data = _allListings.ToList();

            // SEARCH
            string keyword = txtSearch.Text.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                data = data.Where(x => x.Title.ToLower().Contains(keyword) || x.ApproximateAddress.ToLower().Contains(keyword) || x.HostName.ToLower().Contains(keyword)).ToList();
            }

            // TYPE
            if (cbTypeFilter.SelectedItem != null && cbTypeFilter.SelectedItem.ToString() != "All")
            {
                string type = cbTypeFilter.SelectedItem.ToString();
                data = data.Where(x => x.Type == type).ToList();
            }

            // STATUS
            if (cbStatusFilter.SelectedItem != null && cbStatusFilter.SelectedItem.ToString() != "All")
            {
                string status = cbStatusFilter.SelectedItem.ToString();
                data = data.Where(x => x.Status == status).ToList();
            }

            // SORT

            switch (cbSort.SelectedItem?.ToString())
            {
                case "A-Z":
                    data = data.OrderBy(x => x.Title).ToList();
                    break;

                case "Price":
                    data = data.OrderByDescending(x => x.MinPrice).ToList();
                    break;

                default:
                    data = data.OrderByDescending(x => x.ID).ToList();
                    break;
            }

            RenderListings(data);
        }

        // OPEN
        private void OpenListing(DTO_ItemCard item) => MessageBox.Show($"Open listing detail:\n{item.Title}");

        // ADD
        private void btnAddListing_Click(object sender, RoutedEventArgs e)
        {
            OpenOverlay();
            listingEditorDrawer.Open();
            listingEditorDrawer.LoadListing(null);
            LoadDrawerData();
        }


        // EDIT
        private void EditListing(DTO_ItemCard item)
        {
            _selectedListing = item;
            OpenOverlay();
            listingEditorDrawer.Open();
            ET_Items entity = _busItems.GetEditItems(item.ID);
            listingEditorDrawer.LoadListing(entity);
            LoadDrawerData(item.ID);
        }

        private void LoadDrawerData(long itemId = 0)
        {

            // AMENITIES
            List<DTO_Amenities> amenities =
                _busAmenity.GetData(itemId);


            // ATTRACTIONS
            List<DTO_Attraction_Distance> attractions = _busAttraction.GetData(itemId);


            // PICTURES
            List<ET_ItemPictures> pictures = _busPictures.LoadPictures(itemId);


            // LOAD UI
            listingEditorDrawer.LoadAmenities(amenities);
            listingEditorDrawer.LoadAttractions(attractions);
            listingEditorDrawer.LoadPictures(pictures);
        }


        // DELETE
        private void DeleteListing(DTO_ItemCard item)
        {
            var confirm = MessageBox.Show($"Delete '{item.Title}' ?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            bool result = _busItems.Delete(item.ID);

            if (result)
            {
                MessageBox.Show("Deleted.");

                LoadListings();
            }
            else
            {
                MessageBox.Show("Delete failed.");
            }
        }


        // CALENDAR


        private void OpenCalendar(DTO_ItemCard item)
        {
            ListingCalendarWindow window = new ListingCalendarWindow();

            window.LoadListing(item.ID, item.Title, item.ApproximateAddress);

            window.ShowDialog();
            LoadListings();
        }


        // DRAWER EVENTS
        private void ListingEditorDrawer_CloseClicked(object sender, EventArgs e) => CloseOverlay();

        private void ListingEditorDrawer_SaveClicked(object sender, EventArgs e)
        {
            try
            {
                ET_Items item = listingEditorDrawer.GetItem();

                // AMENITIES               
                var amenityIds = listingEditorDrawer.GetSelectedAmenities().Select(x => x.ID).ToList();


                // ATTRACTIONS
                var attractions = listingEditorDrawer.GetSelectedAttractions();

                bool result;

                // ADD               
                if (item.ID == 0)

                    result = _busItems.Add(item, amenityIds, attractions);

                // UPDATE               
                else
                {
                    result = _busItems.Update(item, amenityIds, attractions);
                }

                // RESULT
                if (result)
                {
                    var pictures = listingEditorDrawer.GetPictures();
                    int coverIndex = listingEditorDrawer.GetCoverIndex();

                    // Đưa ảnh cover lên đầu (DisplayOrder = 0) → sẽ thành thumbnail
                    if (coverIndex > 0 && coverIndex < pictures.Count)
                    {
                        var cover = pictures[coverIndex];
                        pictures.RemoveAt(coverIndex);
                        pictures.Insert(0, cover);
                    }

                    _busPictures.SavePictures(item.ID, pictures);
                    MessageBox.Show("Listing saved successfully.");
                    CloseOverlay();
                    LoadListings();
                }
                else
                    MessageBox.Show(_busItems.LastError);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ListingEditorDrawer_DeleteClicked(object sender, EventArgs e)
        {
            if (_selectedListing == null)
                return;

            DeleteListing(_selectedListing);
            CloseOverlay();
        }


        // OVERLAY
        private void OpenOverlay() => OverlayPanel.Visibility = Visibility.Visible;


        private void CloseOverlay()
        {
            OverlayPanel.Visibility = Visibility.Collapsed;

            listingEditorDrawer.Close();
        }


        // FILTER EVENTS
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void cbTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void cbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        // REFRESH
        private void btnRefresh_Click(object sender, RoutedEventArgs e) => LoadListings();

    }
}