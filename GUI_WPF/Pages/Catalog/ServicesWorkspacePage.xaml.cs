using BUS;
using ET;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.Pages.Catalog
{
    public partial class ServicesWorkspacePage : Page
    {
        // BUS
        private readonly BUS_ServiceType _busType =
            new BUS_ServiceType();

        private readonly BUS_Services _busService =
            new BUS_Services();

        // DATA
        private List<ET_ServiceType> _types =
            new List<ET_ServiceType>();

        private List<ET_Services> _services =
            new List<ET_Services>();

        private ET_ServiceType _selectedType;

        private ET_Services _selectedService;

        
        // MODE
        

        private bool _isCreatingType = false;

        private bool _isCreatingService = false;

        
        // IMAGE
        

        private string _selectedIconPath = null;

        
        // CONSTRUCTOR
        

        public ServicesWorkspacePage()
        {
            InitializeComponent();

            LoadServiceTypes();

            LoadServiceTypeComboBox();
        }

        
        // LOAD TYPES
        

        private void LoadServiceTypes()
        {
            _types = _busType.GetData();

            lstServiceTypes.ItemsSource =
                _types;

            if (_types.Any())
            {
                lstServiceTypes.SelectedIndex = 0;
            }
        }

        
        // LOAD SERVICES
        

        private void LoadServices(long typeId)
        {
            _services =
                _busService.GetByType(typeId);

            lstServices.ItemsSource =
                _services;

            txtServiceCount.Text =
                $"{_services.Count} Services";
        }

        
        // LOAD COMBO
        

        private void LoadServiceTypeComboBox()
        {
            cboServiceType.ItemsSource =
                _busType.GetData();

            cboServiceType.DisplayMemberPath =
                "Name";

            cboServiceType.SelectedValuePath =
                "ID";
        }

        
        // SELECT TYPE
        

        private void lstServiceTypes_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (lstServiceTypes.SelectedItem == null)
                return;

            _selectedType =
                (ET_ServiceType)
                lstServiceTypes.SelectedItem;

            txtSelectedType.Text =
                _selectedType.Name;

            LoadServices(_selectedType.ID);

            ShowTypeInspector();
        }

        
        // SELECT SERVICE
        

        private void lstServices_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (lstServices.SelectedItem == null)
                return;

            _selectedService =
                (ET_Services)
                lstServices.SelectedItem;

            ShowServiceInspector();
        }

        
        // SHOW TYPE INSPECTOR
        

        private void ShowTypeInspector()
        {
            pnlTypeInspector.Visibility =
                Visibility.Visible;

            pnlServiceInspector.Visibility =
                Visibility.Collapsed;

            txtTypeInspectorTitle.Text =
                _selectedType.Name;

            txtTypeName.Text =
                _selectedType.Name;

            txtTypeDescription.Text =
                _selectedType.Description;

            _isCreatingType = false;
        }

        
        // SHOW SERVICE INSPECTOR
        

        private void ShowServiceInspector()
        {
            pnlTypeInspector.Visibility =
                Visibility.Collapsed;

            pnlServiceInspector.Visibility =
                Visibility.Visible;

            txtServiceInspectorTitle.Text =
                _selectedService.Name;

            txtServiceName.Text =
                _selectedService.Name;

            cboServiceType.SelectedValue =
                _selectedService.ServiceTypeID;

            txtPrice.Text =
                _selectedService.Price.ToString();

            txtDuration.Text =
                _selectedService.Duration?.ToString();

            txtDayOfWeek.Text =
                _selectedService.DayOfWeek;

            txtDayOfMonth.Text =
                _selectedService.DayOfMonth;

            txtDailyCap.Text =
                _selectedService.DailyCap.ToString();

            txtBookingCap.Text =
                _selectedService.BookingCap.ToString();

            txtServiceDescription.Text =
                _selectedService.Description;

            _isCreatingService = false;
        }

        
        // CREATE TYPE
        

        private void btnCreateType_Click(
            object sender,
            RoutedEventArgs e)
        {
            _isCreatingType = true;

            _selectedType = null;

            pnlTypeInspector.Visibility =
                Visibility.Visible;

            pnlServiceInspector.Visibility =
                Visibility.Collapsed;

            txtTypeInspectorTitle.Text =
                "Create Service Type";

            txtTypeName.Text = "";

            txtTypeDescription.Text = "";

            _selectedIconPath = null;

            lstServiceTypes.SelectedItem = null;

            txtTypeName.Focus();
        }

        
        // CREATE SERVICE
        

        private void btnCreateService_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_selectedType == null)
            {
                MessageBox.Show(
                    "Please select a service type.");

                return;
            }

            _isCreatingService = true;

            _selectedService = null;

            pnlTypeInspector.Visibility =
                Visibility.Collapsed;

            pnlServiceInspector.Visibility =
                Visibility.Visible;

            txtServiceInspectorTitle.Text =
                "Create Service";

            txtServiceName.Text = "";

            cboServiceType.SelectedValue =
                _selectedType.ID;

            txtPrice.Text = "";

            txtDuration.Text = "";

            txtDayOfWeek.Text = "";

            txtDayOfMonth.Text = "";

            txtDailyCap.Text = "1";

            txtBookingCap.Text = "1";

            txtServiceDescription.Text = "";

            lstServices.SelectedItem = null;

            txtServiceName.Focus();
        }

        
        // SAVE TYPE
        

        private void btnSaveType_Click(
            object sender,
            RoutedEventArgs e)
        {
            string name =
                txtTypeName.Text.Trim();

            string description =
                txtTypeDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    "Type name is required.");

                return;
            }

            bool exists =
                _busType.IsNameExists(
                    name,
                    _selectedType?.ID ?? 0);

            if (exists)
            {
                MessageBox.Show(
                    "Type name already exists.");

                return;
            }

             
            // CREATE
             

            if (_isCreatingType)
            {
                ET_ServiceType type =
                    new ET_ServiceType
                    {
                        Name = name,
                        Description = description
                    };

                bool result =
                    _busType.Insert(
                        type,
                        _selectedIconPath);

                if (result)
                {
                    MessageBox.Show(
                        "Service type created.");

                    LoadServiceTypes();
                }
                else
                {
                    MessageBox.Show(
                        "Create failed.");
                }

                return;
            }

             
            // UPDATE
             

            if (_selectedType == null)
                return;

            _selectedType.Name = name;

            _selectedType.Description =
                description;

            bool updateResult =
                _busType.Update(
                    _selectedType,
                    _selectedIconPath);

            if (updateResult)
            {
                MessageBox.Show(
                    "Service type updated.");

                long selectedId =
                    _selectedType.ID;

                LoadServiceTypes();

                var selected =
                    _types.FirstOrDefault(x =>
                        x.ID == selectedId);

                if (selected != null)
                {
                    lstServiceTypes.SelectedItem =
                        selected;
                }
            }
            else
            {
                MessageBox.Show(
                    "Update failed.");
            }
        }

        
        // DELETE TYPE
        

        private void btnDeleteType_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_selectedType == null)
                return;

            MessageBoxResult confirm =
                MessageBox.Show(
                    $"Delete '{_selectedType.Name}' ?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            bool result =
                _busType.Delete(_selectedType.ID);

            if (result)
            {
                MessageBox.Show(
                    "Deleted successfully.");

                LoadServiceTypes();
            }
            else
            {
                MessageBox.Show(
                    "Cannot delete type with services.");
            }
        }

        
        // SAVE SERVICE
        

        private void btnSaveService_Click(
            object sender,
            RoutedEventArgs e)
        {
            string name =
                txtServiceName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    "Service name required.");

                return;
            }

            if (!decimal.TryParse(
                txtPrice.Text,
                out decimal price))
            {
                MessageBox.Show(
                    "Invalid price.");

                return;
            }

            if (!long.TryParse(
                txtDailyCap.Text,
                out long dailyCap))
            {
                MessageBox.Show(
                    "Invalid daily cap.");

                return;
            }

            if (!long.TryParse(
                txtBookingCap.Text,
                out long bookingCap))
            {
                MessageBox.Show(
                    "Invalid booking cap.");

                return;
            }

            if (bookingCap > dailyCap)
            {
                MessageBox.Show(
                    "Booking cap cannot exceed daily cap.");

                return;
            }

            long? duration = null;

            if (!string.IsNullOrWhiteSpace(
                txtDuration.Text))
            {
                if (long.TryParse(
                    txtDuration.Text,
                    out long d))
                {
                    duration = d;
                }
            }

            long typeId =
                Convert.ToInt64(
                    cboServiceType.SelectedValue);

             
            // CREATE
             

            if (_isCreatingService)
            {
                ET_Services service =
                    new ET_Services
                    {
                        ServiceTypeID = typeId,
                        Name = name,
                        Price = price,
                        Duration = duration,
                        Description =
                            txtServiceDescription.Text.Trim(),
                        DayOfWeek =
                            txtDayOfWeek.Text.Trim(),
                        DayOfMonth =
                            txtDayOfMonth.Text.Trim(),
                        DailyCap = dailyCap,
                        BookingCap = bookingCap
                    };

                bool insertResult =
                    _busService.Insert(service);

                if (insertResult)
                {
                    MessageBox.Show(
                        "Service created.");

                    LoadServices(_selectedType.ID);
                }
                else
                {
                    MessageBox.Show(
                        "Create failed.");
                }

                return;
            }

             
            // UPDATE
             

            if (_selectedService == null)
                return;

            _selectedService.ServiceTypeID =
                typeId;

            _selectedService.Name = name;

            _selectedService.Price = price;

            _selectedService.Duration =
                duration;

            _selectedService.Description =
                txtServiceDescription.Text.Trim();

            _selectedService.DayOfWeek =
                txtDayOfWeek.Text.Trim();

            _selectedService.DayOfMonth =
                txtDayOfMonth.Text.Trim();

            _selectedService.DailyCap =
                dailyCap;

            _selectedService.BookingCap =
                bookingCap;

            bool updateResult =
                _busService.Update(
                    _selectedService);

            if (updateResult)
            {
                MessageBox.Show(
                    "Service updated.");

                long selectedId =
                    _selectedService.ID;

                LoadServices(_selectedType.ID);

                var selected =
                    _services.FirstOrDefault(x =>
                        x.ID == selectedId);

                if (selected != null)
                {
                    lstServices.SelectedItem =
                        selected;
                }
            }
            else
            {
                MessageBox.Show(
                    "Update failed.");
            }
        }

        
        // DELETE SERVICE
        

        private void btnDeleteService_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_selectedService == null)
                return;

            MessageBoxResult confirm =
                MessageBox.Show(
                    $"Delete '{_selectedService.Name}' ?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            bool result =
                _busService.Delete(
                    _selectedService.ID);

            if (result)
            {
                MessageBox.Show(
                    "Deleted successfully.");

                LoadServices(_selectedType.ID);

                pnlTypeInspector.Visibility =
                    Visibility.Visible;

                pnlServiceInspector.Visibility =
                    Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show(
                    "Delete failed.");
            }
        }

        
        // SEARCH TYPE
        

        private void txtSearchType_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            string keyword =
                txtSearchType.SearchText
                .Trim()
                .ToLower();

            lstServiceTypes.ItemsSource =
                _types.Where(x =>
                    x.Name.ToLower()
                    .Contains(keyword))
                .ToList();
        }

        
        // SEARCH SERVICE
        

        private void txtSearchService_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            string keyword =
                txtSearchService.SearchText
                .Trim()
                .ToLower();

            lstServices.ItemsSource =
                _services.Where(x =>
                    x.Name.ToLower()
                    .Contains(keyword))
                .ToList();
        }

        
        // SORT SERVICE
        

        private void cboSortService_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cboSortService.SelectedItem == null)
                return;

            if (!_services.Any())
                return;

            ComboBoxItem item =
                (ComboBoxItem)
                cboSortService.SelectedItem;

            string sort =
                item.Content.ToString();

            List<ET_Services> sorted =
                new List<ET_Services>();

            switch (sort)
            {
                case "Newest":

                    sorted = _services
                        .OrderByDescending(x => x.ID)
                        .ToList();

                    break;

                case "A-Z":

                    sorted = _services
                        .OrderBy(x => x.Name)
                        .ToList();

                    break;

                case "Price High":

                    sorted = _services
                        .OrderByDescending(x => x.Price)
                        .ToList();

                    break;

                case "Price Low":

                    sorted = _services
                        .OrderBy(x => x.Price)
                        .ToList();

                    break;

                case "Capacity":

                    sorted = _services
                        .OrderByDescending(x => x.DailyCap)
                        .ToList();

                    break;

                default:

                    sorted = _services;

                    break;
            }

            lstServices.ItemsSource =
                sorted;
        }

        
        // FILTER SERVICE
        

        private void cboFilterService_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cboFilterService.SelectedItem == null)
                return;

            ComboBoxItem item =
                (ComboBoxItem)
                cboFilterService.SelectedItem;

            string filter =
                item.Content.ToString();

            IEnumerable<ET_Services> query =
                _services;

            switch (filter)
            {
                case "Recurring":

                    query = query.Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.DayOfWeek));

                    break;

                case "Limited":

                    query = query.Where(x =>
                        x.DailyCap <= 10);

                    break;

                case "High Capacity":

                    query = query.Where(x =>
                        x.DailyCap >= 50);

                    break;
            }

            lstServices.ItemsSource =
                query.ToList();
        }

        
        // CHOOSE ICON
        

        private void btnChooseIcon_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog =
                new OpenFileDialog();

            dialog.Filter =
                "Image Files|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                _selectedIconPath =
                    dialog.FileName;

                MessageBox.Show(
                    "Icon selected.");
            }
        }

        
        // SERVICE TYPE CHANGED
        

        private void cboServiceType_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {

        }
    }
}