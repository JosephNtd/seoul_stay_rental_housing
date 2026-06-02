using BUS;
using ET;
using GUI_WPF.UserControls.Cards;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GUI_WPF.Pages.Catalog
{
    public partial class PropertyTypesPage : Page
    {

        // BUS
        private readonly BUS_ItemType _bus =
            new BUS_ItemType();

        // DATA
        private List<ET_ItemTypes> _itemTypes =
            new List<ET_ItemTypes>();

        private ET_ItemTypes _selectedItemType;

        // MODE
        private bool _isCreating = false;

        // IMAGE
        private string _selectedIconPath = null;

        // CONSTRUCTOR
        public PropertyTypesPage()
        {
            try
            {
                InitializeComponent();
                drawerInspector.SaveClicked += drawerInspector_SaveClicked;

                drawerInspector.DeleteClicked += drawerInspector_DeleteClicked;

                drawerInspector.ChooseIconClicked += drawerInspector_ChooseIconClicked;
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        // LOAD DATA
        private void LoadData()
        {
            _itemTypes =
                _bus.GetData();

            RenderCards(_itemTypes);
        }

        // RENDER CARDS


        private void RenderCards(List<ET_ItemTypes> data)
        {
            wpPropertyTypes.Children.Clear();

            foreach (var item in data)
            {
                PropertyTypeCard card = null;
                try
                {
                    card = new PropertyTypeCard();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                // DATA

                card.Title = item.Name ?? "";
                card.Description = item.Description ?? "";

                int count = _bus.GetItemsByItemTypeID(item.ID).Count();

                card.ListingCount = $"{count} Listings";

                // EMOJI

                switch ((item.Name ?? "").ToLower())
                {
                    case "apartment":
                        card.Emoji = "🏢";
                        break;

                    case "house":
                        card.Emoji = "🏠";
                        break;

                    case "boutique hotel":
                        card.Emoji = "🏨";
                        break;

                    case "unique space":
                        card.Emoji = "🛖";
                        break;

                    default:
                        card.Emoji = "🏡";
                        break;
                }

                // TAG

                card.Tag = item;

                // EVENTS

                card.EditClicked += Card_EditClicked;

                card.PreviewClicked += Card_PreviewClicked;

                // UI

                wpPropertyTypes.Children.Add(card);
            }
        }

        //  EDIT

        private void Card_EditClicked(object sender, RoutedEventArgs e)
        {
            PropertyTypeCard card = (PropertyTypeCard)sender;

            _selectedItemType = (ET_ItemTypes)card.Tag;

            ShowInspector();
        }

        // PREVIEW

        private void Card_PreviewClicked(object sender, RoutedEventArgs e)
        {
            PropertyTypeCard card = (PropertyTypeCard)sender;

            ET_ItemTypes item = (ET_ItemTypes)card.Tag;

            MessageBox.Show($"Guest Preview\n\n{item.Name}\n\n{item.Description}", "Guest Preview");
        }


        // SHOW INSPECTOR


        private void ShowInspector()
        {
            drawerInspector.pnlEmptyState.Visibility =
                Visibility.Collapsed;

            drawerInspector.pnlInspector.Visibility =
                Visibility.Visible;

            drawerInspector.txtTitle.Text =
                "Edit Property Type";

            drawerInspector.txtName.Text =
                _selectedItemType.Name;

            drawerInspector.txtDescription.Text =
                _selectedItemType.Description;


            // PREVIEW


            drawerInspector.txtPreviewName.Text =
                _selectedItemType.Name;

            drawerInspector.txtPreviewDescription.Text =
                _selectedItemType.Description;


            // IMAGE


            if (!string.IsNullOrWhiteSpace(
                _selectedItemType.IconPath)
                &&
                File.Exists(
                    _selectedItemType.IconPath))
            {
                BitmapImage bitmap =
                    new BitmapImage();

                bitmap.BeginInit();

                bitmap.UriSource =
                    new Uri(
                        _selectedItemType.IconPath,
                        UriKind.Absolute);

                bitmap.CacheOption =
                    BitmapCacheOption.OnLoad;

                bitmap.EndInit();

                drawerInspector.imgPreview.Source =
                    bitmap;
            }

            _isCreating = false;
        }


        // CREATE


        private void btnCreateType_Click(object sender, RoutedEventArgs e)
        {
            _isCreating = true;

            _selectedItemType = null;

            drawerInspector.pnlEmptyState.Visibility = Visibility.Collapsed;

            drawerInspector.pnlInspector.Visibility = Visibility.Visible;

            drawerInspector.txtTitle.Text = "Create Property Type";

            drawerInspector.txtName.Text = "";

            drawerInspector.txtDescription.Text = "";

            drawerInspector.txtPreviewName.Text = "Property Type";

            drawerInspector.txtPreviewDescription.Text = "Description preview";

            drawerInspector.imgPreview.Source = null;

            _selectedIconPath = null;
        }


        // SAVE
        private void drawerInspector_SaveClicked(object sender, RoutedEventArgs e)
        {
            string name = drawerInspector.txtName.Text.Trim();

            string description = drawerInspector.txtDescription.Text.Trim();


            // VALIDATION

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Property type name is required.");

                return;
            }

            bool exists =
                _bus.IsNameExists(name, _selectedItemType?.ID ?? 0);

            if (exists)
            {
                MessageBox.Show("Property type already exists.");

                return;
            }


            // CREATE


            if (_isCreating)
            {
                ET_ItemTypes item =
                    new ET_ItemTypes
                    {
                        Name = name,
                        Description = description
                    };

                bool result =
                    _bus.Insert(
                        item,
                        GetFileName(_selectedIconPath));

                if (result)
                {
                    MessageBox.Show(
                        "Property type created.");

                    LoadData();
                }
                else
                {
                    MessageBox.Show(
                        "Create failed.");
                }

                return;
            }


            // UPDATE


            if (_selectedItemType == null)
                return;

            _selectedItemType.Name =
                name;

            _selectedItemType.Description =
                description;

            bool updateResult =
                _bus.Update(
                    _selectedItemType,
                    GetFileName(_selectedIconPath));

            if (updateResult)
            {
                MessageBox.Show(
                    "Updated successfully.");

                LoadData();
            }
            else
            {
                MessageBox.Show(
                    "Update failed.");
            }
        }


        // DELETE


        private void drawerInspector_DeleteClicked(object sender, RoutedEventArgs e)
        {
            if (_selectedItemType == null)
                return;

            MessageBoxResult confirm =
                MessageBox.Show(
                    $"Delete '{_selectedItemType.Name}' ?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            bool result = _bus.Delete(_selectedItemType.ID);

            if (result)
            {
                MessageBox.Show("Deleted successfully.");

                LoadData();

                drawerInspector.pnlInspector.Visibility = Visibility.Collapsed;

                drawerInspector.pnlEmptyState.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Cannot delete. Property type is being used.");
            }
        }


        // CHOOSE ICON


        private void drawerInspector_ChooseIconClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                _selectedIconPath = dialog.FileName;

                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();

                bitmap.UriSource = new Uri(dialog.FileName, UriKind.Absolute);

                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                bitmap.EndInit();

                drawerInspector.imgPreview.Source = bitmap;
            }
        }


        // SEARCH


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.SearchText.Trim().ToLower();

            var filtered = _itemTypes.Where(x => (x.Name ?? "").ToLower().Contains(keyword)).ToList();

            RenderCards(filtered);
        }


        // SORT


        private void cboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_itemTypes == null || !_itemTypes.Any())
                return;

            if (cboSort.SelectedItem == null)
                return;

            ComboBoxItem item = cboSort.SelectedItem as ComboBoxItem;

            if (item == null)
                return;

            string sort =
                item.Content?.ToString() ?? "";

            List<ET_ItemTypes> sorted = new List<ET_ItemTypes>();

            switch (sort)
            {
                case "A-Z":
                    sorted = _itemTypes.OrderBy(x => x.Name ?? "").ToList();
                    break;

                case "Newest":
                    sorted = _itemTypes.OrderByDescending(x => x.ID).ToList();
                    break;

                default:
                    sorted = _itemTypes;
                    break;
            }

            RenderCards(sorted);
        }


        // VIEW MODE


        private void cboViewMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        // PREVIEW GUEST


        private void btnPreviewGuest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Guest preview mode coming soon.");
        }


        // GET FILE NAME


        private string GetFileName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return Path.GetFileName(path);
        }
    }
}
