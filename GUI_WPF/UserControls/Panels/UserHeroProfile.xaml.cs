using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI_WPF.UserControls.Panels
{
    public partial class UserHeroProfile : UserControl
    {
        public UserHeroProfile()
        {
            InitializeComponent();
        }

         
        // EVENTS
         

        public event RoutedEventHandler EditClicked;

        public event RoutedEventHandler LockClicked;

         
        // BASIC INFO
         

        public string FullName
        {
            get => txtFullName.Text;
            set
            {
                txtFullName.Text = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] parts = value.Split(' ');

                    if (parts.Length >= 2)
                    {
                        txtInitials.Text =
                            $"{parts[0][0]}{parts[1][0]}";
                    }
                }
            }
        }

        public string Username
        {
            get => txtUsername.Text;
            set => txtUsername.Text = value;
        }

        public string Email
        {
            get => txtEmail.Text;
            set => txtEmail.Text = value;
        }

        public string Phone
        {
            get => txtPhone.Text;
            set => txtPhone.Text = value;
        }

        public string Country
        {
            get => txtCountry.Text;
            set => txtCountry.Text = value;
        }

        public string JoinedDate
        {
            get => txtJoined.Text;
            set => txtJoined.Text = value;
        }

         
        // ROLE
         

        public string Role
        {
            get => txtRole.Text;
            set
            {
                txtRole.Text = value;

                switch (value)
                {
                    case "Administrator":

                        RoleChip.Background =
                            new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString("#FFF6E7"));

                        txtRole.Foreground =
                            new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString("#D29B22"));

                        break;

                    case "Host":

                        RoleChip.Background =
                            new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString("#FCECEC"));

                        txtRole.Foreground =
                            new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString("#C97D7D"));

                        break;

                    default:

                        RoleChip.Background =
                            new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString("#EEF3FF"));

                        txtRole.Foreground =
                            new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString("#6484D6"));

                        break;
                }
            }
        }

         
        // VERIFIED
         

        public string Verification
        {
            get => txtVerified.Text;
            set => txtVerified.Text = value;
        }

         
        // STATUS
         

        public string Status
        {
            get => txtStatus.Text;
            set => txtStatus.Text = value;
        }

         
        // HOST INFO
         

        public string HostInfo
        {
            get => txtHostInfo.Text;
            set => txtHostInfo.Text = value;
        }

        public string BusinessInfo
        {
            get => txtBusiness.Text;
            set => txtBusiness.Text = value;
        }

         
        // GUEST INFO
         

        public string GuestInfo
        {
            get => txtGuestInfo.Text;
            set => txtGuestInfo.Text = value;
        }

        public string LanguageInfo
        {
            get => txtLanguage.Text;
            set => txtLanguage.Text = value;
        }

         
        // AVATAR
         

        public void SetAvatar(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            BitmapImage bitmap =
                new BitmapImage();

            bitmap.BeginInit();

            bitmap.UriSource =
                new System.Uri(imagePath);

            bitmap.CacheOption =
                BitmapCacheOption.OnLoad;

            bitmap.EndInit();

            imgAvatar.Source = bitmap;

            txtInitials.Visibility =
                Visibility.Collapsed;
        }

         
        // EVENTS
         

        private void btnEdit_Click(
            object sender,
            RoutedEventArgs e)
        {
            EditClicked?.Invoke(this, e);
        }

        private void btnLock_Click(
            object sender,
            RoutedEventArgs e)
        {
            LockClicked?.Invoke(this, e);
        }
    }
}
