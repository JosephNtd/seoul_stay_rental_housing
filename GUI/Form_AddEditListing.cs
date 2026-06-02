using BUS;
using DevExpress.Utils.Html;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraTab;
using DTO;
using ET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DangNhap_Form
{
    public partial class Form_AddEditListing : Form
    {
        BUS_Amenity _amenity;
        BUS_Attraction _attraction;
        BUS_Items _items;
        BUS_Area _areas;
        BindingList<DTO_Amenities> _amenityData;
        BindingList<DTO_Attraction_Distance> _attractionData;
        private BUS_ItemPictures _busPictures = new BUS_ItemPictures();
        private List<ET_ItemPictures> _pictureList = new List<ET_ItemPictures>();
        private string imageDir = Path.Combine(Application.StartupPath, "Images"); // thư mục lưu ảnh
        long _hostUserId;
        bool _allowTabChange;

        public ET_Items _et { get; set; }
        public bool Mode { get; set; }

        public Form_AddEditListing(ET_Items et, bool mode)
        {
            InitializeComponent();
            InitializeServices();
            _et = et;
            Mode = mode;
            _hostUserId = et.HostUserID;
        }

        public Form_AddEditListing(long hostUserId, bool mode)
        {
            InitializeComponent();
            InitializeServices();
            _hostUserId = hostUserId;
            Mode = mode;
        }

        public Form_AddEditListing(bool mode) : this(0, mode) { }

        private void InitializeServices()
        {
            _amenity = new BUS_Amenity();
            _attraction = new BUS_Attraction();
            _items = new BUS_Items();
            _areas = new BUS_Area();
            _busPictures = new BUS_ItemPictures();
            tabControl.SelectedPageChanging += tabControl_SelectedPageChanging;
            tabControl.SelectedPageChanged += tabControl_SelectedPageChanged;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (Mode) // Edit mode: lưu và đóng
            {
                SaveListing();
            }
            else
            {
                Close();
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            SaveListing();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            var index = tabControl.SelectedTabPageIndex;

            // Validate từng tab trước khi qua tab tiếp theo
            if (index == 0 && !ValidateListingDetails())
                return;
            if (index == 1 && !ValidateAmenities())
                return;
            if (index == 2 && !ValidateAttractions())
                return;

            MoveToTab(Math.Min(index + 1, tabControl.TabPages.Count - 1));
        }

        private void gcAmenity_Load(object sender, EventArgs e)
        {
            ConfigureAmenityGrid();
        }

        private void gcAttraction_Load(object sender, EventArgs e)
        {
            ConfigureAttractionGrid();
        }

        private void SummaryFooter(GridView gridView)
        {
            if (gridView.Columns.Count == 0) return;
            var column = gridView.Columns["Name"] ?? gridView.Columns["Attraction"];
            if (column == null)
                column = gridView.VisibleColumns.Count > 0 ? gridView.VisibleColumns[0] : gridView.Columns[0];
            column.Summary.Clear();
            column.Summary.Add(DevExpress.Data.SummaryItemType.Count, column.FieldName, "{0} items found.");
        }

        private void Form_AddEditListing_Load(object sender, EventArgs e)
        {
            PrepareControls();
            LoadData();
            LoadListing();
            ConfigureMode();
            UpdateNavigationButtons();
            
            ConfigurePictureTileView(); // Cấu hình hiển thị ảnh
            LoadPictures();
        }

        private void LoadData()
        {
            // Item Types
            cbType.Properties.DataSource = _items.GetItemTypes();
            cbType.Properties.DisplayMember = "Name";
            cbType.Properties.ValueMember = "ID";
            cbType.Properties.PopulateColumns();
            if (cbType.Properties.Columns["ID"] != null) cbType.Properties.Columns["ID"].Visible = false;
            if (cbType.Properties.Columns["GUID"] != null) cbType.Properties.Columns["GUID"].Visible = false;
            if (cbType.Properties.Columns["IconPath"] != null) cbType.Properties.Columns["IconPath"].Visible = false;
            if (cbType.Properties.Columns["Description"] != null) cbType.Properties.Columns["Description"].Visible = false;
            cbType.Properties.NullText = "-- Select type --";
            cbType.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;

            // Approximate Address (Areas)
            cbApproxAdress.Properties.DataSource = _areas.GetAreasName();
            cbApproxAdress.Properties.DisplayMember = "Name";
            cbApproxAdress.Properties.ValueMember = "ID";
            cbApproxAdress.Properties.PopulateColumns();
            if (cbApproxAdress.Properties.Columns["ID"] != null) cbApproxAdress.Properties.Columns["ID"].Visible = false;
            if (cbApproxAdress.Properties.Columns["GUID"] != null) cbApproxAdress.Properties.Columns["GUID"].Visible = false;
            cbApproxAdress.Properties.NullText = "-- Select area --";
            cbApproxAdress.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
        }

        private void PrepareControls()
        {
            numCapacity.Minimum = 0; numBed.Minimum = 0; numBedroom.Minimum = 0; numBathroom.Minimum = 0;
            numTimeMinimum.Minimum = 0; numTimeMaximum.Minimum = 0;

            numCapacity.Maximum = 10000; numBed.Maximum = 10000; numBedroom.Maximum = 10000;
            numBathroom.Maximum = 10000; numTimeMinimum.Maximum = 10000; numTimeMaximum.Maximum = 10000;

            if (!Mode)
            {
                numCapacity.Value = 1;
                numBed.Value = 1;
                numBedroom.Value = 1;
                numBathroom.Value = 1;
                numTimeMinimum.Value = 1;
                numTimeMaximum.Value = 365;
            }
        }

        private void LoadListing()
        {
            long? itemId = Mode && _et != null ? (long?)_et.ID : null;

            _amenityData = new BindingList<DTO_Amenities>(_amenity.GetData(itemId));
            _attractionData = new BindingList<DTO_Attraction_Distance>(_attraction.GetData(itemId));
            gcAmenity.DataSource = _amenityData;
            gcAttraction.DataSource = _attractionData;
            ConfigureAmenityGrid();
            ConfigureAttractionGrid();

            if (!Mode || _et == null) return;

            cbType.EditValue = _et.ItemTypeID;
            txtTitle.Text = _et.Title;
            numCapacity.Value = _et.Capacity;
            numBed.Value = _et.NumberOfBeds;
            numBathroom.Value = _et.NumberOfBathrooms;
            numBedroom.Value = _et.NumberOfBedrooms;
            txtExactAddress.Text = _et.ExactAddress;
            cbApproxAdress.Text = _et.ApproximateAddress;
            txtDescription.Text = _et.Description;
            txtHostRule.Text = _et.HostRules;
            numTimeMaximum.Value = _et.MaximumNights;
            numTimeMinimum.Value = _et.MinimumNights;
        }

        private void ConfigureMode()
        {
            Text = Mode && _et != null ? $"Edit Listing - {_et.Title}" : "Add Listing";
            if (!Mode) tabControl.SelectedTabPageIndex = 0;
        }

        private void ConfigureAmenityGrid()
        {
            if (gvAmenity.Columns.Count == 0) gvAmenity.PopulateColumns();
            if (gvAmenity.Columns.Count == 0) return;
            SummaryFooter(gvAmenity);
            gvAmenity.OptionsBehavior.Editable = true;
            gvAmenity.OptionsBehavior.ReadOnly = false;
            if (gvAmenity.Columns["ID"] != null) gvAmenity.Columns["ID"].Visible = false;
            if (gvAmenity.Columns["Name"] != null)
            {
                gvAmenity.Columns["Name"].OptionsColumn.AllowEdit = false;
                gvAmenity.Columns["Name"].OptionsColumn.ReadOnly = true;
            }
            if (gvAmenity.Columns["IsSelected"] != null)
            {
                gvAmenity.Columns["IsSelected"].Caption = "Available";
                gvAmenity.Columns["IsSelected"].ColumnEdit = repositoryItemCheckEdit1;
                gvAmenity.Columns["IsSelected"].OptionsColumn.AllowEdit = true;
                gvAmenity.Columns["IsSelected"].OptionsColumn.ReadOnly = false;
                gvAmenity.Columns["IsSelected"].VisibleIndex = 0;
            }
            gvAmenity.BestFitColumns();
        }

        private void ConfigureAttractionGrid()
        {
            if (gvAttraction.Columns.Count == 0) gvAttraction.PopulateColumns();
            if (gvAttraction.Columns.Count == 0) return;
            SummaryFooter(gvAttraction);
            gvAttraction.OptionsBehavior.Editable = true;
            gvAttraction.OptionsBehavior.ReadOnly = false;
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvAttraction.Columns)
                col.OptionsColumn.AllowEdit = false;
            if (gvAttraction.Columns["AttractionID"] != null) gvAttraction.Columns["AttractionID"].Visible = false;
            SetEditableAttractionColumn("Distance", "Distance (km)");
            SetEditableAttractionColumn("OnFoot", "On Foot (minutes)");
            SetEditableAttractionColumn("ByCar", "By Car (minutes)");
            gvAttraction.BestFitColumns();
        }

        private void SetEditableAttractionColumn(string fieldName, string caption)
        {
            var column = gvAttraction.Columns[fieldName];
            if (column == null) return;
            column.Caption = caption;
            column.OptionsColumn.AllowEdit = true;
            column.OptionsColumn.ReadOnly = false;
        }

        private void tabControl_SelectedPageChanging(object sender, TabPageChangingEventArgs e)
        {
            if (!Mode && !_allowTabChange && e.Page != tabControl.SelectedTabPage)
                e.Cancel = true;
        }

        private void tabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            UpdateNavigationButtons();
        }

        private void MoveToTab(int index)
        {
            _allowTabChange = true;
            tabControl.SelectedTabPageIndex = index;
            _allowTabChange = false;
            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            if (Mode) // Edit mode: chỉ có nút Close
            {
                btnClose.Visible = true;
                btnCancel.Visible = false;
                btnNext.Visible = false;
                btnFinish.Visible = false;
                return;
            }

            // Add mode: wizard
            var index = tabControl.SelectedTabPageIndex;
            int lastTabIndex = tabControl.TabPages.Count - 1; // 3 – tab Pictures cuối cùng

            btnClose.Visible = false;
            btnCancel.Visible = index < lastTabIndex;
            btnNext.Visible = index < lastTabIndex;
            btnFinish.Visible = index == lastTabIndex;
        }

        // ===================== TAB PICTURES =====================
        private void LoadPictures()
        {
            if (Mode)
                _pictureList = _busPictures.LoadPictures(_et.ID);
            else
                _pictureList = new List<ET_ItemPictures>();

            if (!Directory.Exists(imageDir))
                Directory.CreateDirectory(imageDir);

            gcPictures.DataSource = _pictureList;

            tvPictures.PopulateColumns();

            if (tvPictures.Columns["ImageDisplay"] != null)
                tvPictures.Columns["ImageDisplay"].Visible = false;
            tvPictures.Columns["FullPath"].Visible = false;
            tvPictures.Columns["IsNew"].Visible = false;
            tvPictures.Columns["ID"].Visible = false;
        }
        
        private void ConfigurePictureTileView()
        {
            // Cấu hình kích thước tile
            tvPictures.OptionsTiles.ItemSize = new Size(220, 240);
            tvPictures.OptionsTiles.Padding = new Padding(8);
            tvPictures.OptionsTiles.IndentBetweenItems = 12;

            // Gắn sự kiện xóa ảnh
            tvPictures.HtmlElementMouseClick -= TvPictures_HtmlElementMouseClick;
            tvPictures.HtmlElementMouseClick += TvPictures_HtmlElementMouseClick;
        }

        private void TvPictures_HtmlElementMouseClick(object sender, TileViewHtmlElementMouseEventArgs e)
        {
            if (e.ElementId == "deleteBtn")
            {
                var pic = tvPictures.GetRow(e.RowHandle) as ET_ItemPictures;
                if (pic != null && MessageBox.Show("Xóa ảnh này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (File.Exists(pic.FullPath))
                        File.Delete(pic.FullPath);
                    _pictureList.Remove(pic);
                    gcPictures.RefreshDataSource();
                }
            }
        }

        private void btnAddPicture_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in ofd.FileNames)
                    {
                        string ext = Path.GetExtension(file);
                        string newFileName = $"{Guid.NewGuid()}{ext}";
                        string destPath = Path.Combine(imageDir, newFileName);
                        File.Copy(file, destPath, true);

                        _pictureList.Add(new ET_ItemPictures
                        {
                            FileName = newFileName,
                            FullPath = destPath,
                            DisplayOrder = _pictureList.Count,
                            IsNew = true
                        });
                    }
                    gcPictures.RefreshDataSource();
                }
            }
        }
         ======

        private bool SaveListing()
        {
            CommitGridEditors();

            if (!ValidateListingDetails() || !ValidateAmenities() || !ValidateAttractions())
                return false;

            var item = BuildItemFromForm();
            var amenityIds = GetSelectedAmenityIds();
            var attractions = _attractionData?.ToList() ?? new List<DTO_Attraction_Distance>();
            bool success = Mode ? _items.Update(item, amenityIds, attractions)
                                : _items.Add(item, amenityIds, attractions);

            if (!success)
            {
                MessageBox.Show(_items.LastError ?? "Unable to save listing.",
                    "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Sau khi có item.ID, lưu ảnh
            _busPictures.SavePictures(item.ID, _pictureList);

            _et = item;
            DialogResult = DialogResult.OK;
            Close();
            return true;
        }

        private ET_Items BuildItemFromForm()
        {
            return new ET_Items
            {
                ID = _et?.ID ?? 0,
                GUID = _et?.GUID ?? Guid.Empty,
                HostUserID = _hostUserId,
                ItemTypeID = Convert.ToInt64(cbType.EditValue),
                AreaID = _items.GetAreaIdByApproximateAddress(cbApproxAdress.Text.Trim()),
                Title = txtTitle.Text.Trim(),
                Capacity = Convert.ToInt32(numCapacity.Value),
                NumberOfBeds = Convert.ToInt32(numBed.Value),
                NumberOfBedrooms = Convert.ToInt32(numBedroom.Value),
                NumberOfBathrooms = Convert.ToInt32(numBathroom.Value),
                ExactAddress = txtExactAddress.Text.Trim(),
                ApproximateAddress = cbApproxAdress.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                HostRules = txtHostRule.Text.Trim(),
                MinimumNights = Convert.ToInt32(numTimeMinimum.Value),
                MaximumNights = Convert.ToInt32(numTimeMaximum.Value)
            };
        }

        private List<long> GetSelectedAmenityIds()
        {
            CommitGridEditors();
            return (_amenityData ?? new BindingList<DTO_Amenities>())
                .Where(x => x.IsSelected)
                .Select(x => x.ID)
                .ToList();
        }


        private bool ValidateListingDetails()
        {
            if (!Mode && _hostUserId <= 0)
                return ShowValidation("Cannot identify the current owner for this listing.");
            if (cbType.EditValue == null)
                return ShowValidation("Please select a listing type.", cbType);
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
                return ShowValidation("Please enter the listing title.", txtTitle);
            if (numCapacity.Value <= 0 || numBed.Value <= 0 || numBedroom.Value <= 0 || numBathroom.Value <= 0)
                return ShowValidation("Capacity, beds, bedrooms and bathrooms must be greater than zero.");
            if (string.IsNullOrWhiteSpace(cbApproxAdress.Text))
                return ShowValidation("Please enter the approximate address.", cbApproxAdress);
            if (_items.GetAreaIdByApproximateAddress(cbApproxAdress.Text.Trim()) <= 0)
                return ShowValidation("Approximate Address must match an existing area.", cbApproxAdress);
            if (string.IsNullOrWhiteSpace(txtExactAddress.Text))
                return ShowValidation("Please enter the exact address.", txtExactAddress);
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
                return ShowValidation("Please enter the description.", txtDescription);
            if (string.IsNullOrWhiteSpace(txtHostRule.Text))
                return ShowValidation("Please enter the host rules.", txtHostRule);
            if (numTimeMinimum.Value <= 0 || numTimeMinimum.Value > numTimeMaximum.Value)
                return ShowValidation("Minimum nights must be greater than zero and less than or equal to maximum nights.");
            return true;
        }

        private bool ValidateAmenities() { CommitGridEditors(); return true; }

        private bool ValidateAttractions()
        {
            CommitGridEditors();
            var attractions = _attractionData ?? new BindingList<DTO_Attraction_Distance>();
            foreach (var attr in attractions)
            {
                if (attr.Distance.HasValue && attr.Distance.Value <= 0)
                    return ShowValidation("Distance must be greater than zero.");
                if ((attr.OnFoot.HasValue || attr.ByCar.HasValue) && !attr.Distance.HasValue)
                    return ShowValidation("Please enter distance before entering travel time.");
                if (attr.OnFoot.HasValue && attr.OnFoot.Value < 0)
                    return ShowValidation("On foot duration cannot be negative.");
                if (attr.ByCar.HasValue && attr.ByCar.Value < 0)
                    return ShowValidation("By car duration cannot be negative.");
            }
            if (!Mode && attractions.Count(x => x.Distance.HasValue) < 2)
                return ShowValidation("Please enter the distance for at least two attractions.");
            return true;
        }

        private void CommitGridEditors()
        {
            gvAmenity.CloseEditor(); gvAmenity.UpdateCurrentRow();
            gvAttraction.CloseEditor(); gvAttraction.UpdateCurrentRow();
        }

        private bool ShowValidation(string message, Control control = null)
        {
            MessageBox.Show(message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            control?.Focus();
            return false;
        }

        private void tvPictures_ItemClick(object sender, TileViewItemClickEventArgs e)
        {
            var pic = tvPictures.GetRow(e.Item.RowHandle) as ET_ItemPictures;
            if (pic != null && MessageBox.Show("Remove this picture?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (File.Exists(pic.FullPath)) File.Delete(pic.FullPath);
                _pictureList.Remove(pic);
                gcPictures.RefreshDataSource();
            }
        }
    }
}