using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Cards
{
    public partial class UserDirectoryCard : UserControl
    {
        public UserDirectoryCard()
        {
            InitializeComponent();
        }

        // EVENTS
         

        public event RoutedEventHandler Selected;

         
        // PROPERTIES
         

        public string FullName
        {
            get => txtName.Text;
            set
            {
                txtName.Text = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] parts = value.Split(' ');

                    if (parts.Length >= 2)
                    {
                        txtInitials.Text =
                            $"{parts[0][0]}{parts[1][0]}";
                    }
                    else
                    {
                        txtInitials.Text =
                            value.Substring(0, 1);
                    }
                }
            }
        }

        public string Email
        {
            get => txtEmail.Text;
            set => txtEmail.Text = value;
        }

        public string Role
        {
            get => txtRole.Text;
            set
            {
                txtRole.Text = value;

                switch (value)
                {
                    case "Admin":

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

        public string Status
        {
            get => txtStatus.Text;
            set
            {
                txtStatus.Text = value;

                if (value == "Active")
                {
                    StatusChip.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#E8F8F0"));

                    txtStatus.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#47A87B"));
                }
                else
                {
                    StatusChip.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FDECEC"));

                    txtStatus.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D96A6A"));
                }
            }
        }

        public string Country
        {
            get => txtCountry.Text;
            set => txtCountry.Text = value;
        }

        public string BookingText
        {
            get => txtBookings.Text;
            set => txtBookings.Text = value;
        }

         
        // SELECTED STATE
         

        public bool IsSelected
        {
            set
            {
                if (value)
                {
                    RootBorder.BorderBrush =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#E6B8B8"));

                    RootBorder.BorderThickness =
                        new Thickness(2);
                }
                else
                {
                    RootBorder.BorderThickness =
                        new Thickness(0);
                }
            }
        }

         
        // CLICK
         

        private void RootBorder_MouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            Selected?.Invoke(this, new RoutedEventArgs());
        }
    }
}
