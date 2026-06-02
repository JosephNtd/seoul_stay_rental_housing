using BUS;
using DTO;
using System.Windows;

namespace GUI_WPF.Pages.Bookings
{
    public partial class CreateGuestDialog : Window
    {
        private readonly BUS_User _busUser = new BUS_User();

        public CreateGuestDialog()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Full name is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Email is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Password is required."); return;
            }

            DTO_User dto = new DTO_User
            {
                FullName = fullName,
                Email = email,
                Username = username,
                Password = password
            };

            bool success = _busUser.InsertUser(dto, "Guest");

            if (!success)
            {
                MessageBox.Show("Unable to create guest.");
                return;
            }

            MessageBox.Show("Guest created successfully.");
            DialogResult = true;
            Close();
        }
    }
}