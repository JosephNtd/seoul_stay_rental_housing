using GUI_WPF.Helpers;
using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.Views
{
    public partial class DatabaseSettingsWindow : Window
    {
        public DatabaseSettingsWindow()
        {
            InitializeComponent();
            Loaded += DatabaseSettingsWindow_Loaded;
            rbWindowsAuth.Checked += Authentication_Checked;
            rbSqlAuth.Checked += Authentication_Checked;
        }
        private void DatabaseSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadServers();
            LoadCurrentConnection();
        }
        private void LoadServers()
        {
            try
            {
                cbServer.Items.Clear();

                DataTable table = SqlDataSourceEnumerator.Instance.GetDataSources();

                foreach (DataRow row in table.Rows)
                {
                    string server = row["ServerName"].ToString();

                    string instance = row["InstanceName"].ToString();

                    if (!string.IsNullOrWhiteSpace(instance))
                    {
                        server += "\\" + instance;
                    }

                    cbServer.Items.Add(server);
                }
                if (cbServer.Items.Count == 0)
                {
                    cbServer.Items.Add(@".\SQLEXPRESS");
                    cbServer.Items.Add("localhost");
                    cbServer.Items.Add("(local)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadDatabases()
        {
            try
            {
                cbDatabase.Items.Clear();

                string server = cbServer.Text;

                if (string.IsNullOrWhiteSpace(server))
                    return;

                string conn = $"Server={server};Integrated Security=True;TrustServerCertificate=True;";

                using (SqlConnection sql = new SqlConnection(conn))
                {
                    sql.Open();

                    DataTable databases = sql.GetSchema("Databases");

                    foreach (DataRow row in databases.Rows)
                    {
                        string dbName = row["database_name"].ToString();

                        cbDatabase.Items.Add(dbName);
                    }
                }
            }
            catch
            {
                // bỏ qua nếu chưa kết nối được server
            }
        }
        private void cbServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDatabases();

            txtConnectionString.Text = BuildConnectionString();

            cbServer.Text = cbServer.SelectedItem.ToString();
        }
        private void ConnectionInput_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                txtConnectionString.Text = BuildConnectionString();
                cbDatabase.Text = cbDatabase.SelectedItem.ToString();
            }
            catch
            {
            }
        }

        #region Load

        private void LoadCurrentConnection()
        {
            try
            {
                string conn = ConnectionManager.CurrentConnectionString;

                if (string.IsNullOrWhiteSpace(conn))
                    return;

                var builder = new SqlConnectionStringBuilder(conn);

                cbServer.Text = builder.DataSource;

                cbDatabase.Text = builder.InitialCatalog;

                bool isWindowsAuth = builder.IntegratedSecurity;
                rbWindowsAuth.IsChecked = isWindowsAuth;
                rbSqlAuth.IsChecked = !isWindowsAuth;
                txtUsername.Text = builder.UserID;
                txtPassword.Password = builder.Password;

                UpdateAuthenticationState();

                txtConnectionString.Text = conn;
            }
            catch
            {
                // bỏ qua lỗi parse
            }
        }

        #endregion

        #region Authentication

        private void Authentication_Checked(object sender, RoutedEventArgs e)
        {
            UpdateAuthenticationState();

            txtConnectionString.Text = BuildConnectionString();
        }

        private void UpdateAuthenticationState()
        {
            if (cbServer == null || cbDatabase == null || rbSqlAuth == null || txtUsername == null || txtPassword == null)
            {
                return;
            }

            bool sqlAuth =
                rbSqlAuth.IsChecked == true;

            txtUsername.IsEnabled = sqlAuth;
            txtPassword.IsEnabled = sqlAuth;
        }

        #endregion

        #region Connection String

        private string BuildConnectionString()
        {
            // THÊM ĐOẠN KIỂM TRA NÀY ĐỂ TRÁNH SẬP APP KHI MỞ FORM
            if (rbWindowsAuth == null || txtUsername == null || txtPassword == null)
            {
                return string.Empty;
            }

            string server = cbServer.Text.Trim();
            string database = cbDatabase.Text.Trim();
            bool windowsAuth = rbWindowsAuth.IsChecked == true;

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                TrustServerCertificate = true // Giúp chạy mượt mà trên máy khác không có SSL xịn
            };

            if (windowsAuth)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.IntegratedSecurity = false;
                builder.UserID = txtUsername.Text.Trim();
                builder.Password = txtPassword.Password;
            }

            return builder.ConnectionString;
        }

        #endregion

        #region Test

        private void btnTestConnection_Click(object sender,
            RoutedEventArgs e)
        {
            try
            {
                string conn = BuildConnectionString();
                txtConnectionString.Text = conn;
                using (SqlConnection sql = new SqlConnection(conn))
                {
                    sql.Open();
                }

                txtStatus.Text = "✓ Connection successful";
                txtStatus.Foreground = System.Windows.Media.Brushes.SeaGreen;

                MessageBox.Show("Connection successful.", "Database", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                txtStatus.Text = "✗ Connection failed";

                txtStatus.Foreground = System.Windows.Media.Brushes.Firebrick;

                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Save

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string conn = BuildConnectionString();

                ConnectionManager.SaveConnectionString(conn);

                txtConnectionString.Text = conn;

                MessageBox.Show("Database settings saved successfully.\n\nPlease restart the application.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Close

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}