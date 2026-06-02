using BUS;
using DTO;
using ET;
using GUI_WPF.UserControls.Cards;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Drawers
{
    public partial class ListingEditorDrawer : UserControl
    {
        public ListingEditorDrawer()
        {
            InitializeComponent();

            Visibility = Visibility.Collapsed;

            LoadComboboxData();
        }

        // DATA
        private ET_Items _currentItem;
        private readonly List<DTO_Amenities> _selectedAmenities = new List<DTO_Amenities>();
        private readonly List<DTO_Attraction_Distance> _selectedAttractions = new List<DTO_Attraction_Distance>();
        private readonly List<ET_ItemPictures> _pictures = new List<ET_ItemPictures>();
        private readonly BUS_ItemType _busItemType = new BUS_ItemType();
        private readonly BUS_Area _busArea = new BUS_Area();
        private Button _currentActiveTab;
        private int _coverIndex = 0;


        // EVENTS
        public event EventHandler SaveClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler CloseClicked;


        // PUBLIC GETTERS

        private void LoadComboboxData()
        {
            try
            {
                // ITEM TYPES
                cbPropertyTypes.ItemsSource = _busItemType.GetData();
                // AREAS
                cbAreas.ItemsSource = _busArea.GetAreasName();
                // DEFAULT SELECT
                if (cbPropertyTypes.Items.Count > 0)
                {
                    cbPropertyTypes.SelectedIndex = 0;
                }
                if (cbAreas.Items.Count > 0)
                {
                    cbAreas.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Combobox Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public ET_Items GetItem()
        {
            if (_currentItem == null)

                _currentItem = new ET_Items();


            _currentItem.Title = txtTitle.Text.Trim();
            _currentItem.ApproximateAddress = txtApproximateAddress.Text.Trim();
            _currentItem.ExactAddress = txtExactAddress.Text.Trim();
            _currentItem.Description = txtDescription.Text.Trim();
            _currentItem.HostRules = txtHostRules.Text.Trim();

            // TYPE
            if (cbPropertyTypes.SelectedValue != null)
                _currentItem.ItemTypeID = Convert.ToInt64(cbPropertyTypes.SelectedValue);

            // AREA
            if (cbAreas.SelectedValue != null)
                _currentItem.AreaID = Convert.ToInt64(cbAreas.SelectedValue);

            // NUMBERS
            int.TryParse(txtCapacity.Text, out int capacity);
            _currentItem.Capacity = capacity;
            int.TryParse(txtBeds.Text, out int beds);
            _currentItem.NumberOfBeds = beds;
            int.TryParse(txtBedrooms.Text, out int bedrooms);
            _currentItem.NumberOfBedrooms = bedrooms;
            int.TryParse(txtBathrooms.Text, out int bathrooms);
            _currentItem.NumberOfBathrooms = bathrooms;
            int.TryParse(txtMinimumNights.Text, out int minNights);
            _currentItem.MinimumNights = minNights;
            int.TryParse(txtMaximumNights.Text, out int maxNights);
            _currentItem.MaximumNights = maxNights;

            return _currentItem;
        }

        public List<DTO_Amenities> GetSelectedAmenities() => _selectedAmenities;
        public List<DTO_Attraction_Distance> GetSelectedAttractions() => _selectedAttractions;
        public List<ET_ItemPictures> GetPictures() => _pictures;
        public int GetCoverIndex() => _coverIndex;

        // LOAD DROPDOWNS
        public void LoadPropertyTypes(object data) => cbPropertyTypes.ItemsSource = data as System.Collections.IEnumerable;
        public void LoadAreas(object data) => cbAreas.ItemsSource = data as System.Collections.IEnumerable;

        // LOAD LISTING
        public void LoadListing(ET_Items item)
        {
            _currentItem = item;

            if (item == null)
            {
                txtDrawerMode.Text = "Create New Listing";
                ClearForm();
                return;
            }

            txtDrawerMode.Text = "Edit Listing";

            // BASIC
            txtTitle.Text = item.Title;
            txtApproximateAddress.Text = item.ApproximateAddress;
            txtExactAddress.Text = item.ExactAddress;
            txtDescription.Text = item.Description;
            txtHostRules.Text = item.HostRules;

            // COMBO
            cbPropertyTypes.SelectedValue = item.ItemTypeID;
            cbAreas.SelectedValue = item.AreaID;

            // CONFIG
            txtCapacity.Text = item.Capacity.ToString();
            txtBeds.Text = item.NumberOfBeds.ToString();
            txtBedrooms.Text = item.NumberOfBedrooms.ToString();
            txtBathrooms.Text = item.NumberOfBathrooms.ToString();

            // RULES
            txtMinimumNights.Text = item.MinimumNights.ToString();
            txtMaximumNights.Text = item.MaximumNights.ToString();
        }


        // CLEAR
        private void ClearForm()
        {
            txtTitle.Clear();
            txtApproximateAddress.Clear();
            txtExactAddress.Clear();
            txtDescription.Clear();
            txtHostRules.Clear();

            txtCapacity.Clear();
            txtBeds.Clear();
            txtBedrooms.Clear();
            txtBathrooms.Clear();

            txtMinimumNights.Clear();
            txtMaximumNights.Clear();

            cbPropertyTypes.SelectedIndex = -1;
            cbAreas.SelectedIndex = -1;

            wpAmenities.Children.Clear();
            spAttractions.Children.Clear();
            wpPictures.Children.Clear();

            _selectedAmenities.Clear();
            _selectedAttractions.Clear();
            _pictures.Clear();
        }

        // TABS
        private void ShowPanel(UIElement panel)
        {
            DetailsPanel.Visibility = Visibility.Collapsed;
            AmenitiesPanel.Visibility = Visibility.Collapsed;
            AttractionsPanel.Visibility = Visibility.Collapsed;
            PicturesPanel.Visibility = Visibility.Collapsed;
            panel.Visibility = Visibility.Visible;
        }

        private void btnTabDetails_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel(DetailsPanel);
            SetActiveTab(btnTabDetails);
        }

        private void btnTabAmenities_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel(AmenitiesPanel);
            SetActiveTab(btnTabAmenities);
        }

        private void btnTabAttractions_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel(AttractionsPanel);
            SetActiveTab(btnTabAttractions);
        }

        private void btnTabPictures_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel(PicturesPanel);
            SetActiveTab(btnTabPictures);
        }

        // AMENITIES
        public void LoadAmenities(List<DTO_Amenities> amenities)
        {
            wpAmenities.Children.Clear();

            if (amenities == null)
                return;

            foreach (var amenity in amenities)
            {
                var chip = new Cards.AmenityChip();

                chip.Width = 180;
                chip.Height = 110;
                chip.Margin = new Thickness(0, 0, 20, 20);

                chip.LoadAmenity(amenity);

                chip.SelectionChanged += (s, e) =>
                    {
                        if (amenity.IsSelected)
                        {
                            if (!_selectedAmenities.Any(x => x.ID == amenity.ID))
                            {
                                _selectedAmenities.Add(amenity);
                            }
                        }
                        else
                        {
                            _selectedAmenities.RemoveAll(x => x.ID == amenity.ID);
                        }
                    };

                wpAmenities.Children.Add(chip);

                if (amenity.IsSelected)
                {
                    _selectedAmenities.Add(amenity);
                }
            }
        }


        // ATTRACTIONS
        public void LoadAttractions(
            List<DTO_Attraction_Distance> attractions)
        {
            spAttractions.Children.Clear();

            _selectedAttractions.Clear();

            if (attractions == null)
                return;

            foreach (var attraction in attractions)
            {
                AddAttractionRow(attraction);
            }
        }

        private void AddAttractionRow(
            DTO_Attraction_Distance attraction)
        {
            var row = new Cards.AttractionRow();

            row.Margin = new Thickness(0, 0, 0, 18);

            row.LoadAttraction(attraction);

            row.RemoveClicked += (s, e) =>
                {
                    spAttractions.Children.Remove(row);

                    _selectedAttractions.RemoveAll(x => x.AttractionID == attraction.AttractionID);
                };

            spAttractions.Children.Add(row);

            _selectedAttractions.Add(attraction);
        }

        private void btnAddAttraction_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open attraction picker window here.");
        }


        // PICTURES
        public void LoadPictures(
            List<ET_ItemPictures> pictures)
        {
            wpPictures.Children.Clear();

            _pictures.Clear();

            if (pictures == null)
                return;

            _pictures.AddRange(pictures);

            RenderPictures();
        }

        private void RenderPictures()
        {
            wpPictures.Children.Clear();

            for (int i = 0; i < _pictures.Count; i++)
            {
                var picture = _pictures[i];

                var tile = new Cards.PictureTile();

                tile.Margin = new Thickness(0, 0, 18, 18);

                tile.LoadPicture(picture, i == _coverIndex);

                int index = i;

                // DELETE
                tile.DeleteClicked += (s, e) =>
                    {
                        _pictures.RemoveAt(index);

                        if (_coverIndex >= _pictures.Count)
                        {
                            _coverIndex = 0;
                        }

                        RenderPictures();
                    };

                // COVER
                tile.SetCoverClicked += (s, e) =>
                    {
                        _coverIndex = index;
                        RenderPictures();
                    };

                wpPictures.Children.Add(tile);
            }
        }

        private void btnUploadPictures_Click(object sender, RoutedEventArgs e)
        {
            UploadPictures();
        }
        private string GetImagesFolder()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Listings");
        }

        private void UploadPictures()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Images|*.png;*.jpg;*.jpeg;*.webp";

            if (dialog.ShowDialog() != true) return;

            string imagesFolder = GetImagesFolder();
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            foreach (string sourceFile in dialog.FileNames)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(sourceFile)}"; // tên unique
                string destPath = Path.Combine(imagesFolder, fileName);

                File.Copy(sourceFile, destPath, true); // copy file

                var picture = new ET_ItemPictures
                {
                    FileName = fileName,
                    FullPath = destPath,
                    DisplayOrder = _pictures.Count,
                    IsNew = true
                };

                _pictures.Add(picture);
            }

            RenderPictures();
        }

        // DRAG DROP
        private void DropZone_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            string imagesFolder = GetImagesFolder();
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            foreach (string sourceFile in files)
            {
                if (!File.Exists(sourceFile))
                    continue;

                string ext = Path.GetExtension(sourceFile).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".webp")
                    continue;

                // Tạo tên file unique để tránh trùng
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(sourceFile)}";
                string destPath = Path.Combine(imagesFolder, fileName);

                try
                {
                    File.Copy(sourceFile, destPath, true); // Copy file vào thư mục Images

                    var picture = new ET_ItemPictures
                    {
                        FileName = fileName,           // Chỉ lưu tên file
                        FullPath = destPath,           // Đường dẫn đầy đủ trong project
                        DisplayOrder = _pictures.Count,
                        IsNew = true
                    };

                    _pictures.Add(picture);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể copy file {Path.GetFileName(sourceFile)}: {ex.Message}",
                                  "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            RenderPictures();
        }

        // SAVE
        public bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Title is required.");
                return false;
            }

            if (cbPropertyTypes.SelectedIndex < 0)
            {
                MessageBox.Show("Select property type.");
                return false;
            }

            if (cbAreas.SelectedIndex < 0)
            {
                MessageBox.Show("Select area.");
                return false;
            }

            return true;
        }

        public void Save()
        {
            if (!ValidateData())
                return;
            SaveClicked?.Invoke(this, EventArgs.Empty);
        }

        // BUTTONS
        private void btnDeleteListing_Click(object sender, RoutedEventArgs e) => DeleteClicked?.Invoke(this, EventArgs.Empty);
        private void btnClose_Click(object sender, RoutedEventArgs e) => CloseClicked?.Invoke(this, EventArgs.Empty);
        private void btnSaveListing_Click(object sender, RoutedEventArgs e)
        {
            foreach (AttractionRow row in spAttractions.Children.OfType<AttractionRow>())
            {
                row.UpdateAttractionData();
            }
            Save();
        }
        private void btnCancelDrawer_Click(object sender, RoutedEventArgs e) => CloseClicked?.Invoke(this, EventArgs.Empty);

        // OPEN / CLOSE
        public void Open() => Visibility = Visibility.Visible;

        public void Close() => Visibility = Visibility.Collapsed;

        private void SetActiveTab(Button activeButton)
        {
            // Reset all tabs to inactive
            foreach (Button btn in TabNavPanel.Children.OfType<Button>())
            {
                if (btn == activeButton)
                    SetTabActive(btn);
                else
                    SetTabInactive(btn);
            }

            _currentActiveTab = activeButton;
        }

        private void SetTabActive(Button btn)
        {
            var border = FindVisualChild<Border>(btn);
            if (border != null)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(123, 84, 85)); // #7B5455
                border.BorderBrush = null;
                border.BorderThickness = new Thickness(0);
            }

            // Find TextBlock inside and set white color
            var textBlocks = FindVisualChildren<TextBlock>(btn);
            foreach (var tb in textBlocks)
            {
                if (tb.Text != "🏠" && tb.Text != "✨" && tb.Text != "📍" && tb.Text != "🖼")
                {
                    tb.Foreground = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void SetTabInactive(Button btn)
        {
            var border = FindVisualChild<Border>(btn);
            if (border != null)
            {
                border.Background = new SolidColorBrush(Colors.White);
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(234, 223, 216)); // #EADFD8
                border.BorderThickness = new Thickness(1);
            }

            var textBlocks = FindVisualChildren<TextBlock>(btn);
            foreach (var tb in textBlocks)
            {
                if (tb.Text != "🏠" && tb.Text != "✨" && tb.Text != "📍" && tb.Text != "🖼")
                {
                    tb.Foreground = new SolidColorBrush(Color.FromRgb(78, 65, 61)); // #4E413D
                }
            }
        }

        // Helper methods
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                    return tChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                    yield return tChild;

                foreach (var descendant in FindVisualChildren<T>(child))
                    yield return descendant;
            }
        }
    }
}