using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Cards
{
    public partial class BankAccountCard : UserControl
    {
        public BankAccountCard()
        {
            InitializeComponent();
        }

         
        // EVENTS
         

        public event RoutedEventHandler EditClicked;

        public event RoutedEventHandler RemoveClicked;

         
        // BANK NAME
         

        public string BankName
        {
            get => txtBankName.Text;
            set => txtBankName.Text = value;
        }

         
        // ACCOUNT NUMBER
         

        public string AccountNumber
        {
            get => txtAccountNumber.Text;
            set => txtAccountNumber.Text = value;
        }

         
        // ACCOUNT HOLDER
         

        public string AccountHolder
        {
            get => txtHolder.Text;
            set => txtHolder.Text = value;
        }

         
        // CREATED DATE
         

        public string CreatedDate
        {
            get => txtCreatedDate.Text;
            set => txtCreatedDate.Text = value;
        }

         
        // PRIMARY
         

        public bool IsPrimary
        {
            set
            {
                PrimaryChip.Visibility =
                    value
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

         
        // VERIFIED
         

        public bool IsVerified
        {
            set
            {
                if (value)
                {
                    VerifiedChip.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#E8F8F0"));

                    txtVerified.Text = "Verified";

                    txtVerified.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#47A87B"));
                }
                else
                {
                    VerifiedChip.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FDECEC"));

                    txtVerified.Text = "Unverified";

                    txtVerified.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D96A6A"));
                }
            }
        }

         
        // ACTIVE
         

        public bool IsActive
        {
            set
            {
                if (value)
                {
                    ActiveChip.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#EEF3FF"));

                    txtActive.Text = "Active";

                    txtActive.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#6484D6"));
                }
                else
                {
                    ActiveChip.Background =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#F4F4F4"));

                    txtActive.Text = "Disabled";

                    txtActive.Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#888888"));
                }
            }
        }

         
        // BANK ICON
         

        public string BankIcon
        {
            get => txtBankIcon.Text;
            set => txtBankIcon.Text = value;
        }

         
        // EVENTS
         

        private void btnEdit_Click(
            object sender,
            RoutedEventArgs e)
        {
            EditClicked?.Invoke(this, e);
        }

        private void btnRemove_Click(
            object sender,
            RoutedEventArgs e)
        {
            RemoveClicked?.Invoke(this, e);
        }
    }
}