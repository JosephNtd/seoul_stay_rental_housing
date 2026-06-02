using BUS;
using DTO;
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

namespace GUI_WPF.Pages.Catalog
{
    /// <summary>
    /// Interaction logic for LocationWorkspacePage.xaml
    /// </summary>
    public partial class LocationWorkspacePage : Page
    {
        private readonly BUS_Area _busArea = new BUS_Area();
        private readonly BUS_Attraction _busAttraction = new BUS_Attraction();

        // DATA
        private List<DTO_AreaDisplay> _areas = new List<DTO_AreaDisplay>();
        private List<DTO_AttractionDisplay> _attractions = new List<DTO_AttractionDisplay>();
        private ET_Areas _selectedArea;
        private ET_Attractions _selectedAttraction;

        private bool _isCreatingArea = false;
        private bool _isCreatingAttraction = false;

        public LocationWorkspacePage()
        {
            InitializeComponent();
            LoadAreas();
        }

        private void LoadAreas()
        {
            _areas = _busArea.GetData();
            lstAreas.ItemsSource = _areas;
            if (_areas.Any())
            {
                lstAreas.SelectedIndex = 0;
            }
        }

        private void LoadAttractions(long areaId)
        {
            _attractions =
                _busAttraction.GetByArea(areaId);

            lstAttractions.ItemsSource =
                _attractions;
        }


        private void lstAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstAreas.SelectedItem == null)
                return;

            DTO_AreaDisplay dto = (DTO_AreaDisplay)lstAreas.SelectedItem;

            _selectedArea = _busArea.GetByID(dto.AreaID);

            txtInspectorTitle.Text = _selectedArea.Name;

            txtInspectorType.Text = "Area Information";

            txtAreaName.Text = _selectedArea.Name;

            LoadAttractions(_selectedArea.ID);

            pnlAreaInspector.Visibility = Visibility.Visible;

            pnlAttractionInspector.Visibility = Visibility.Collapsed;
            txtAreaListings.Text = _busArea.GetItemsByArea(dto.AreaID).Count().ToString();
            txtAreaAttractions.Text = _busAttraction.GetByArea(dto.AreaID).Count().ToString();
        }

        // ATTRACTION SELECTION
        private void lstAttractions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstAttractions.SelectedItem == null)
                return;

            DTO_AttractionDisplay dto = (DTO_AttractionDisplay)lstAttractions.SelectedItem;

            _selectedAttraction = _busAttraction.GetByID(dto.ID);

            txtInspectorTitle.Text = _selectedAttraction.AttractionName;

            txtInspectorType.Text = "Attraction Details";

            txtAttractionName.Text = _selectedAttraction.AttractionName;

            txtAttractionAddress.Text = _selectedAttraction.Address;

            pnlAreaInspector.Visibility = Visibility.Collapsed;

            pnlAttractionInspector.Visibility = Visibility.Visible;
        }


        // SEARCH AREA
        private void txtSearchArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearchArea.SearchText.Trim().ToLower();
            lstAreas.ItemsSource = _areas.Where(a => a.AreaName.ToLower().Contains(keyword)).ToList();
        }
        // SEARCH ATTRACTION
        private void txtSearchAttraction_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearchAttraction.SearchText.Trim().ToLower();
            lstAttractions.ItemsSource = _attractions.Where(a => a.AttractionName.ToLower().Contains(keyword)).ToList();
        }

        // CREATE AREA
        private void btnCreateArea_Click(object sender, RoutedEventArgs e)
        {
            _isCreatingArea = true;

            _isCreatingAttraction = false;

            // CLEAR CURRENT
            _selectedArea = null;

            // SWITCH INSPECTOR
            pnlAreaInspector.Visibility =
                Visibility.Visible;

            pnlAttractionInspector.Visibility =
                Visibility.Collapsed;

            // HEADER
            txtInspectorTitle.Text = "Create New Area";

            txtInspectorType.Text = "Area Information";

            // CLEAR FORM
            txtAreaName.Text = "";

            txtAreaListings.Text = "0";

            txtAreaAttractions.Text = "0";

            // UNSELECT LIST
            lstAreas.SelectedItem = null;

            // FOCUS
            txtAreaName.Focus();
        }

        // CREATE ATTRACTION
        private void btnCreateAttraction_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedArea == null)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            _isCreatingAttraction = true;

            _isCreatingArea = false;

            // CLEAR CURRENT
            _selectedAttraction = null;

            // SWITCH PANEL
            pnlAreaInspector.Visibility =
                Visibility.Collapsed;

            pnlAttractionInspector.Visibility =
                Visibility.Visible;

            // HEADER
            txtInspectorTitle.Text = "Create New Attraction";

            txtInspectorType.Text = $"Area: {_selectedArea.Name}";

            // CLEAR FORM
            txtAttractionName.Text = "";

            txtAttractionAddress.Text = "";

            // UNSELECT LIST
            lstAttractions.SelectedItem = null;

            // FOCUS
            txtAttractionName.Focus();
        }

        // SAVE AREA
        private void btnSaveArea_Click(object sender, RoutedEventArgs e)
        {
            string name =
                txtAreaName.Text.Trim();

            // VALIDATION
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Area name is required.");
                return;
            }

            // CHECK DUPLICATE
            bool exists =
                _busArea.IsNameExists(name, _selectedArea?.ID ?? 0);

            if (exists)
            {
                MessageBox.Show("Area name already exists.");
                return;
            }

             
            // CREATE MODE
             

            if (_isCreatingArea)
            {
                ET_Areas newArea =
                    new ET_Areas
                    {
                        Name = name
                    };

                bool insertResult = _busArea.Insert(newArea);

                if (insertResult)
                {
                    MessageBox.Show("Area created successfully.");

                    LoadAreas();

                    _isCreatingArea = false;

                    // AUTO SELECT
                    var created =
                        _areas.FirstOrDefault(x =>
                            x.AreaName.ToLower() ==
                            name.ToLower());

                    if (created != null)
                    {
                        lstAreas.SelectedItem =
                            created;
                    }
                }
                else
                {
                    MessageBox.Show("Failed to create area.");
                }

                return;
            }

             
            // UPDATE MODE
             

            if (_selectedArea == null)
                return;

            _selectedArea.Name = name;

            bool result =
                _busArea.Update(_selectedArea);

            if (result)
            {
                MessageBox.Show(
                    "Area updated successfully.");

                long selectedId =
                    _selectedArea.ID;

                LoadAreas();

                var selected =
                    _areas.FirstOrDefault(x =>
                        x.AreaID == selectedId);

                if (selected != null)
                {
                    lstAreas.SelectedItem =
                        selected;
                }
            }
            else
            {
                MessageBox.Show(
                    "Update failed.");
            }
        }

        // DELETE AREA
        private void btnDeleteArea_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedArea == null)
                return;
            MessageBoxResult confirm =
                MessageBox.Show($"Delete area '{_selectedArea.Name}' ?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes)
                return;
            bool result = _busArea.Delete(_selectedArea.ID);
            if (result)
            {
                MessageBox.Show("Area deleted.");
                LoadAreas();
            }
            else
            {
                MessageBox.Show("Cannot delete area.");
            }
        }

        // SAVE ATTRACTION
        private void btnSaveAttraction_Click(object sender, RoutedEventArgs e)
        {
            string name = txtAttractionName.Text.Trim();

            string address = txtAttractionAddress.Text.Trim();

            // VALIDATION
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Attraction name is required.");
                return;
            }

            // CREATE MODE

            if (_isCreatingAttraction)
            {
                ET_Attractions attraction =
                    new ET_Attractions
                    {
                        AreaID = _selectedArea.ID,
                        AttractionName = name,
                        Address = address
                    };

                bool insertResult =
                    _busAttraction.Insert(attraction);

                if (insertResult)
                {
                    MessageBox.Show(
                        "Attraction created successfully.");

                    LoadAttractions(_selectedArea.ID);

                    _isCreatingAttraction = false;

                    // REFRESH COUNTER
                    txtAreaAttractions.Text =
                        _busAttraction
                        .GetByArea(_selectedArea.ID)
                        .Count()
                        .ToString();

                    // AUTO SELECT
                    var created =
                        _attractions.FirstOrDefault(x =>
                            x.AttractionName.ToLower() ==
                            name.ToLower());

                    if (created != null)
                    {
                        lstAttractions.SelectedItem =
                            created;
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Failed to create attraction.");
                }

                return;
            }

            // UPDATE MODE

            if (_selectedAttraction == null)
                return;

            _selectedAttraction.AttractionName =
                name;

            _selectedAttraction.Address =
                address;

            bool result = _busAttraction.Update(_selectedAttraction);

            if (result)
            {
                MessageBox.Show("Attraction updated successfully.");

                long selectedId = _selectedAttraction.ID;

                LoadAttractions(_selectedArea.ID);

                var selected = _attractions.FirstOrDefault(x => x.ID == selectedId);

                if (selected != null)
                    lstAttractions.SelectedItem = selected;
            }
            else
                MessageBox.Show("Update failed.");
        }
        // DELETE ATTRACTION
        private void btnDeleteAttraction_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAttraction == null)
                return;
            MessageBoxResult confirm = MessageBox.Show($"Delete attraction '{_selectedAttraction.AttractionName}' ?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes)
                return;
            bool result = _busAttraction.Delete(_selectedAttraction.ID);
            if (result)
            {
                MessageBox.Show("Attraction deleted.");
                LoadAttractions(_selectedArea.ID);
            }
            else
            {
                MessageBox.Show("Delete failed.");
            }
        }
         
        // SORT ATTRACTIONS
         

        private void cboSortAttraction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSortAttraction.SelectedItem == null)
                return;

            if (_attractions == null || !_attractions.Any())
                return;

            ComboBoxItem selectedItem = (ComboBoxItem)cboSortAttraction.SelectedItem;

            string sortType = selectedItem.Content.ToString();

            List<DTO_AttractionDisplay> sorted = new List<DTO_AttractionDisplay>();

            switch (sortType)
            {
                // NEWEST

                case "Newest":

                    sorted = _attractions
                        .OrderByDescending(x => x.ID)
                        .ToList();

                    break;

                // A-Z

                case "A-Z":

                    sorted = _attractions
                        .OrderBy(x => x.AttractionName)
                        .ToList();

                    break;

                // Z-A

                case "Z-A":

                    sorted = _attractions
                        .OrderByDescending(x => x.AttractionName)
                        .ToList();

                    break;

                // MOST LISTINGS

                case "Most Listings":

                    sorted = _attractions
                        .OrderByDescending(x => x.AreaName)
                        .ToList();

                    break;

                // DEFAULT

                default:

                    sorted = _attractions;

                    break;
            }

            lstAttractions.ItemsSource = sorted;
        }

    }
}
