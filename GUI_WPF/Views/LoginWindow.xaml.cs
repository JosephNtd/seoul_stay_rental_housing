using BUS;
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
using System.Windows.Shapes;
using DAL;

namespace GUI_WPF.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        BUS_User busUser = new BUS_User(); 
        private bool isPasswordVisible = false;
        public LoginWindow()
        {
            InitializeComponent();
            CheckDatabaseConnection();

            LoadSavedLogin();
        }
        private void LoadSavedLogin()
        {
            if (Properties.Settings.Default.KeepLogin)
            {
                txtUsername.Text = Properties.Settings.Default.SavedUsername;

                txtPassword.Password = Properties.Settings.Default.SavedPassword;

                chkKeepLogin.IsChecked = true;
            }
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        { 
            string username = txtUsername.Text.Trim(); 
            string password = txtPassword.Password; 

            if (string.IsNullOrWhiteSpace(username)) 
            { 
                txtMessage.Text = "Please enter username."; 
                return; 
            } 
            if (string.IsNullOrWhiteSpace(password)) 
            { 
                txtMessage.Text = "Please enter password."; 
                return; 
            }

            if (chkKeepLogin.IsChecked == true)
            {
                Properties.Settings.Default.SavedUsername = txtUsername.Text;

                // nếu đang show password
                if (isPasswordVisible)
                {
                    Properties.Settings.Default.SavedPassword = txtShowPassword.Text;
                }
                else
                {
                    Properties.Settings.Default.SavedPassword = txtPassword.Password;
                }

                Properties.Settings.Default.KeepLogin = true;
            }
            else
            {
                Properties.Settings.Default.SavedUsername = "";
                Properties.Settings.Default.SavedPassword = "";
                Properties.Settings.Default.KeepLogin = false;
            }

            Properties.Settings.Default.Save();

            ET_Users user = busUser.Login(username, password); 
            if (user == null) 
            { 
                txtMessage.Text = "Invalid username or password."; 
                return; 
            } 
            AdminWindow main = new AdminWindow(user); 
            main.Show(); 
            this.Close(); 
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e) 
        { 
            RegisterWindow register = new RegisterWindow(); 
            register.Show(); 
            this.Close(); 
        }

        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (!isPasswordVisible)
            {
                txtShowPassword.Text = txtPassword.Password;

                txtShowPassword.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;

                btnShowPassword.Content = "🙈";

                isPasswordVisible = true;
            }
            else
            {
                txtPassword.Password = txtShowPassword.Text;

                txtShowPassword.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;

                btnShowPassword.Content = "👁";

                isPasswordVisible = false;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DatabaseSettingsWindow window =
                    new DatabaseSettingsWindow();

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "ERROR");
            }
        }
        private void CheckDatabaseConnection()
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    db.Connection.Open();

                    db.Connection.Close();
                }
            }
            catch
            {
                MessageBox.Show(
                    "Cannot connect to database.\nPlease configure database settings.",
                    "Database Connection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                DatabaseSettingsWindow window =
                    new DatabaseSettingsWindow();

                bool? result =
                    window.ShowDialog();

                if (result != true)
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
