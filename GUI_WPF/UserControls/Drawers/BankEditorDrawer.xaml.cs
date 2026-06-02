using ET;
using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.UserControls.Drawers
{
    /// <summary>
    /// Interaction logic for BankEditorDrawer.xaml
    /// </summary>
    public partial class BankEditorDrawer : UserControl
    {
         
        // EVENTS
         

        public event RoutedEventHandler SaveClicked;

        public event RoutedEventHandler CancelClicked;

        public event RoutedEventHandler CloseClicked;

         
        // STATE
         

        private bool _isCreateMode;

         
        // PROPERTIES
         

        public bool IsCreateMode
        {
            get => _isCreateMode;

            set
            {
                _isCreateMode = value;

                txtDrawerTitle.Text =
                    value
                    ? "Add Bank Account"
                    : "Edit Bank Account";
            }
        }

         
        // CTOR
         

        public BankEditorDrawer()
        {
            InitializeComponent();
        }

         
        // LOAD DATA
         

        public void LoadBank(
            ET_HostBankAccount bank)
        {
            if (bank == null)
                return;

            txtBankName.Text =
                bank.BankName;

            txtAccountHolder.Text =
                bank.AccountHolder;

            txtAccountNumber.Text =
                bank.AccountNumber;

            chkPrimary.IsChecked =
                bank.IsPrimary;

            chkVerified.IsChecked =
                bank.IsVerified;

            chkActive.IsChecked =
                bank.IsActive;
        }

         
        // CLEAR
         

        public void ClearForm()
        {
            txtBankName.Text = "";

            txtAccountHolder.Text = "";

            txtAccountNumber.Text = "";

            txtSwiftCode.Text = "";

            chkPrimary.IsChecked = false;

            chkVerified.IsChecked = false;

            chkActive.IsChecked = true;
        }

         
        // BUILD ENTITY
         

        public ET_HostBankAccount BuildEntity()
        {
            return new ET_HostBankAccount
            {
                BankName =
                    txtBankName.Text.Trim(),

                AccountHolder =
                    txtAccountHolder.Text.Trim(),

                AccountNumber =
                    txtAccountNumber.Text.Trim(),

                IsPrimary =
                    chkPrimary.IsChecked == true,

                IsVerified =
                    chkVerified.IsChecked == true,

                IsActive =
                    chkActive.IsChecked == true
            };
        }

         
        // VALIDATION
         

        public bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(
                txtBankName.Text))
            {
                MessageBox.Show(
                    "Bank name is required.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                txtAccountHolder.Text))
            {
                MessageBox.Show(
                    "Account holder is required.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                txtAccountNumber.Text))
            {
                MessageBox.Show(
                    "Account number is required.");

                return false;
            }

            return true;
        }

         
        // SAVE
         

        private void btnSave_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            SaveClicked?.Invoke(this, e);
        }

         
        // CANCEL
         

        private void btnCancel_Click(
            object sender,
            RoutedEventArgs e)
        {
            CancelClicked?.Invoke(this, e);
        }

         
        // CLOSE
         

        private void btnCloseDrawer_Click(
            object sender,
            RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, e);
        }
    }
}