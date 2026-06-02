using DTO;
using ET;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GUI_WPF.UserControls.Drawers
{
    /// <summary>
    /// Interaction logic for UserEditorDrawer.xaml
    /// </summary>
    public partial class UserEditorDrawer : UserControl
    {
        // EVENTS
        public event RoutedEventHandler SaveClicked;
        public event RoutedEventHandler CancelClicked;
        public event RoutedEventHandler CloseClicked;

        // STATE
        private bool _isCreateMode = false;
        private string _selectedAvatarPath;

        // PUBLIC
        public bool IsCreateMode
        {
            get => _isCreateMode;

            set
            {
                _isCreateMode = value;
                txtDrawerTitle.Text = value ? "Create User" : "Edit User";
                btnSaveUser.Content = value ? "Create User" : "Save Changes";
            }
        }

        // CTOR
        public UserEditorDrawer()
        {
            InitializeComponent();

            cbUserRole.SelectionChanged += cbUserRole_SelectionChanged;
        }

        // LOAD
        private void UserEditorDrawer_Loaded(object sender, RoutedEventArgs e)
        {
            cbUserRole.SelectionChanged += cbUserRole_SelectionChanged;
            UpdateRoleSections();
        }

        // ROLE VISIBILITY
        private void cbUserRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRoleSections();
        }

        private void UpdateRoleSections()
        {
            ComboBoxItem item = cbUserRole.SelectedItem as ComboBoxItem;
            if (item == null)
                return;

            string role = item.Content.ToString();

            // DEFAULT
            HostSection.Visibility = Visibility.Collapsed;
            GuestSection.Visibility = Visibility.Collapsed;

            switch (role)
            {
                case "Host":
                    HostSection.Visibility = Visibility.Visible;
                    break;

                case "Guest":
                    GuestSection.Visibility = Visibility.Visible;
                    break;

                case "Administrator":
                    break;
            }
        }


        // LOAD USER
        public void LoadUser(DTO_UserWorkspace user)
        {
            if (user == null)
                return;

            // PROFILE
            txtUserFullName.Text = user.User.FullName;

            txtUsername.Text = user.User.Username;
            txtUserEmail.Text = user.User.Email;
            txtUserPhone.Text = user.User.PhoneNumber;
            txtUserCountry.Text = user.User.Country;

            // BIRTHDATE
            if (user.User.BirthDate != null)
            {
                dpBirthDate.SelectedDate = user.User.BirthDate;
            }

            // GENDER
            cbGender.SelectedIndex = user.User.Gender;

            // STATUS
            cbUserStatus.SelectedIndex = user.User.IsActive ? 0 : 1;

            // ROLE
            if (user.IsAdmin)
            {
                cbUserRole.SelectedIndex = 2;
            }
            else if (user.IsHost)
            {
                cbUserRole.SelectedIndex = 1;
            }
            else
            {
                cbUserRole.SelectedIndex = 0;
            }

            // GUEST
            if (user.GuestProfile != null)
            {
                txtLoyaltyPoints.Text = user.GuestProfile.LoyaltyPoints.ToString();

                txtNationalID.Text = user.GuestProfile.NationalID;
                chkNationalVerified.IsChecked = user.GuestProfile.NationalIDVerified;
                SelectLanguage(user.GuestProfile.PreferredLanguage);
            }

            // HOST
            if (user.HostProfile != null)
            {
                txtBusinessLicense.Text = user.HostProfile.BusinessLicense;
                txtTaxCode.Text = user.HostProfile.TaxCode;
                chkHostVerified.IsChecked = user.HostProfile.IsVerified;
            }

            // IMAGE
            LoadAvatar(user.User.ProfilePicture);
            UpdateRoleSections();
        }


        // CLEAR FORM
        public void ClearForm()
        {
            txtUserFullName.Text = "";
            txtUsername.Text = "";
            txtUserEmail.Text = "";
            txtUserPhone.Text = "";
            txtUserCountry.Text = "";
            txtTempPassword.Password = "";
            txtBusinessLicense.Text = "";
            txtTaxCode.Text = "";
            txtLoyaltyPoints.Text = "";
            txtNationalID.Text = "";

            dpBirthDate.SelectedDate = null;
            cbGender.SelectedIndex = 0;
            cbUserRole.SelectedIndex = 0;
            cbUserStatus.SelectedIndex = 0;
            cbLanguage.SelectedIndex = 0;
            chkHostVerified.IsChecked = false;
            chkNationalVerified.IsChecked = false;
            chkEmailVerified.IsChecked = false;
            chkPhoneVerified.IsChecked = false;
            chkTwoFactor.IsChecked = false;
            imgAvatar.Source = null;
            _selectedAvatarPath = null;

            UpdateRoleSections();
        }

        // GET USER DTO
        public DTO_UserWorkspace BuildWorkspace()
        {
            DTO_UserWorkspace dto = new DTO_UserWorkspace();

            dto.User = new ET_Users
            {
                FullName = txtUserFullName.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Email = txtUserEmail.Text.Trim(),
                PhoneNumber = txtUserPhone.Text.Trim(),
                Country = txtUserCountry.Text.Trim(),
                BirthDate = dpBirthDate.SelectedDate,
                Gender = (byte)cbGender.SelectedIndex,
                IsActive = cbUserStatus.SelectedIndex == 0,
                ProfilePicture = _selectedAvatarPath
            };

            string role = ((ComboBoxItem)cbUserRole.SelectedItem).Content.ToString();

            dto.IsAdmin = role == "Administrator";
            dto.IsHost = role == "Host";
            dto.IsGuest = role == "Guest";

            // HOST
            if (dto.IsHost)
            {
                dto.HostProfile = new ET_Host
                {
                    BusinessLicense = txtBusinessLicense.Text,
                    TaxCode = txtTaxCode.Text,
                    IsVerified = chkHostVerified.IsChecked == true
                };
            }

            // GUEST
            if (dto.IsGuest)
            {
                int loyalty = 0;
                int.TryParse(txtLoyaltyPoints.Text, out loyalty);

                dto.GuestProfile = new ET_Guest
                {
                    LoyaltyPoints = loyalty,
                    PreferredLanguage = GetLanguageCode(),
                    NationalID = txtNationalID.Text,
                    NationalIDVerified = chkNationalVerified.IsChecked == true
                };
            }

            return dto;
        }


        // AVATAR
        private void btnUploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg";

            bool? result = dialog.ShowDialog();

            if (result != true)
                return;

            _selectedAvatarPath = dialog.FileName;

            LoadAvatar(_selectedAvatarPath);
        }

        private void LoadAvatar(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return;

                if (!File.Exists(path))
                    return;

                BitmapImage image = new BitmapImage();

                image.BeginInit();

                image.UriSource = new Uri(path);

                image.CacheOption = BitmapCacheOption.OnLoad;

                image.EndInit();

                imgAvatar.Source = image;
            }
            catch
            {

            }
        }

        // LANGUAGE
        private void SelectLanguage(
            string code)
        {
            switch (code)
            {
                case "ko":
                    cbLanguage.SelectedIndex = 1;
                    break;

                case "ja":
                    cbLanguage.SelectedIndex = 2;
                    break;

                case "vi":
                    cbLanguage.SelectedIndex = 3;
                    break;

                case "zh":
                    cbLanguage.SelectedIndex = 4;
                    break;

                default:
                    cbLanguage.SelectedIndex = 0;
                    break;
            }
        }

        private string GetLanguageCode()
        {
            ComboBoxItem item = cbLanguage.SelectedItem as ComboBoxItem;

            if (item == null)
                return "en";

            switch (item.Content.ToString())
            {
                case "Korean":
                    return "ko";

                case "Japanese":
                    return "ja";

                case "Vietnamese":
                    return "vi";

                case "Chinese":
                    return "zh";

                default:
                    return "en";
            }
        }

        // VALIDATION
        public bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtUserFullName.Text))
            {
                MessageBox.Show("Full name is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUserEmail.Text))
            {
                MessageBox.Show("Email is required.");
                return false;
            }

            return true;
        }

        // BUTTONS
        private void btnSaveUser_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            SaveClicked?.Invoke(this, e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => CancelClicked?.Invoke(this, e);


        private void btnCloseDrawer_Click(object sender, RoutedEventArgs e) => CloseClicked?.Invoke(this, e);

    }
}