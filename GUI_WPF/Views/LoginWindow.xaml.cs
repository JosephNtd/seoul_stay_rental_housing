using BUS;
using ET;
using GUI_WPF.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using DAL;

namespace GUI_WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly BUS_User _busUser = new BUS_User();
        private bool _isPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();
            CheckDatabaseConnection();
            LoadSavedLogin();
        }

        #region Init

        private void LoadSavedLogin()
        {
            if (Properties.Settings.Default.KeepLogin)
            {
                txtUsername.Text = Properties.Settings.Default.SavedUsername;
                txtPassword.Password = Properties.Settings.Default.SavedPassword;
                chkKeepLogin.IsChecked = true;
            }
        }

        /// <summary>
        /// Kiểm tra DB khi mở app. Dùng ConnectionManager để lấy connection string
        /// đã được lưu bởi DAL_Settings (đọc từ file trước, fallback về app.config).
        /// </summary>
        private void CheckDatabaseConnection()
        {
            try
            {
                using (var db = new Seoul_StayDataContext(ConnectionManager.CurrentConnectionString))
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

                var window = new DatabaseSettingsWindow();
                bool? result = window.ShowDialog();

                if (result != true)
                    Application.Current.Shutdown();
            }
        }

        #endregion

        #region Login

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();

            // Lấy password từ ô đang hiển thị
            string password = _isPasswordVisible
                ? txtShowPassword.Text
                : txtPassword.Password;

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

            // Lưu thông tin đăng nhập nếu được chọn
            SaveKeepLogin(username, password);

            try
            {
                ET_Users user = _busUser.Login(username, password);

                if (user == null)
                {
                    txtMessage.Text = "Invalid username or password.";
                    return;
                }

                var main = new AdminWindow(user);
                main.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                // Bắt mọi lỗi (DB mất kết nối, timeout, v.v.) để tránh crash app
                txtMessage.Text = "Login failed. Please check database connection.";
                MessageBox.Show(
                    $"An error occurred during login:\n\n{ex.Message}",
                    "Login Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SaveKeepLogin(string username, string password)
        {
            if (chkKeepLogin.IsChecked == true)
            {
                Properties.Settings.Default.SavedUsername = username;
                Properties.Settings.Default.SavedPassword = password;
                Properties.Settings.Default.KeepLogin = true;
            }
            else
            {
                Properties.Settings.Default.SavedUsername = "";
                Properties.Settings.Default.SavedPassword = "";
                Properties.Settings.Default.KeepLogin = false;
            }
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Register

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var register = new RegisterWindow();
            register.Show();
            this.Close();
        }

        #endregion

        #region Show / Hide Password

        private void btnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (!_isPasswordVisible)
            {
                txtShowPassword.Text = txtPassword.Password;
                txtShowPassword.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                btnShowPassword.Content = "🙈";
                _isPasswordVisible = true;
            }
            else
            {
                txtPassword.Password = txtShowPassword.Text;
                txtShowPassword.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
                btnShowPassword.Content = "👁";
                _isPasswordVisible = false;
            }
        }

        #endregion

        #region Database Settings

        private void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new DatabaseSettingsWindow();
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR");
            }
        }

        #endregion

        #region Unused stubs

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        #endregion
    }
}