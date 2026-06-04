using GUI_WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceProcess;
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

        #region Server Loading

        private void LoadServers()
        {
            try
            {
                cbServer.Items.Clear();

                // 1. Ưu tiên: phát hiện các SQL Server instance đang chạy trên máy hiện tại
                foreach (var inst in GetLocalSqlInstances())
                    cbServer.Items.Add(inst);

                // 2. Bổ sung: quét mạng LAN (có thể chậm hoặc bị firewall chặn)
                try
                {
                    DataTable table = SqlDataSourceEnumerator.Instance.GetDataSources();
                    foreach (DataRow row in table.Rows)
                    {
                        string server = row["ServerName"].ToString();
                        string instance = row["InstanceName"].ToString();
                        string entry = string.IsNullOrWhiteSpace(instance)
                                            ? server
                                            : $@"{server}\{instance}";

                        if (!cbServer.Items.Contains(entry))
                            cbServer.Items.Add(entry);
                    }
                }
                catch { /* Quét mạng thất bại – bỏ qua */ }

                // 3. Fallback: nếu vẫn rỗng thì thêm danh sách phổ biến
                if (cbServer.Items.Count == 0)
                    AddFallbackServers();
            }
            catch
            {
                AddFallbackServers();
            }
        }

        /// <summary>
        /// Phát hiện các SQL Server instance đang chạy trên máy hiện tại
        /// bằng cách đọc Windows Services (không cần quyền admin, không cần mạng).
        /// Tìm được cả MSSQLSERVER (default) lẫn MSSQL$TEN_INSTANCE (named).
        /// </summary>
        private List<string> GetLocalSqlInstances()
        {
            var result = new List<string>();
            try
            {
                foreach (ServiceController svc in ServiceController.GetServices())
                {
                    if (svc.ServiceName.Equals("MSSQLSERVER", StringComparison.OrdinalIgnoreCase))
                    {
                        // Default instance – kết nối bằng "." hoặc "localhost"
                        result.Add(".");
                        result.Add("localhost");
                    }
                    else if (svc.ServiceName.StartsWith("MSSQL$", StringComparison.OrdinalIgnoreCase))
                    {
                        // Named instance – ví dụ: MSSQL$SQLEXPRESS → .\SQLEXPRESS
                        string name = svc.ServiceName.Substring("MSSQL$".Length);
                        result.Add($@".\{name}");
                        result.Add($@"localhost\{name}");
                    }
                }
            }
            catch { /* ServiceController không khả dụng – bỏ qua */ }
            return result;
        }

        /// <summary>
        /// Danh sách tên server phổ biến khi không phát hiện được instance nào.
        /// Bao gồm cả SQLEXPRESS lẫn MSSQLSERVER để đảm bảo hoạt động trên mọi máy.
        /// </summary>
        private void AddFallbackServers()
        {
            var fallbacks = new[]
            {
                @".\SQLEXPRESS",
                @".\MSSQLSERVER",
                ".",
                "localhost",
                "(local)",
                @"localhost\SQLEXPRESS",
                @"localhost\MSSQLSERVER",
                @"(localdb)\MSSQLLocalDB",
                @"(localdb)\v11.0",
            };

            foreach (var fb in fallbacks)
                if (!cbServer.Items.Contains(fb))
                    cbServer.Items.Add(fb);
        }

        #endregion

        #region Database Loading

        private void LoadDatabases()
        {
            cbDatabase.Items.Clear();

            string serverText = cbServer.Text.Trim();
            if (string.IsNullOrWhiteSpace(serverText))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(BuildServerConnection()))
                {
                    conn.Open();
                    DataTable databases = conn.GetSchema("Databases");

                    var names = databases.Rows
                        .Cast<DataRow>()
                        .Select(r => r["database_name"].ToString())
                        .OrderBy(n => n);

                    foreach (var name in names)
                        cbDatabase.Items.Add(name);
                }

                SetStatus(null, null); // xóa lỗi cũ
            }
            catch (Exception ex)
            {
                // Không popup – hiển thị lỗi ngay trên form để không làm gián đoạn UX
                SetStatus($"⚠ Cannot load databases: {ex.Message}", isError: true);
            }
        }

        #endregion

        #region Connection String Builders

        /// <summary>Connection string tới SERVER (không có database) – dùng khi liệt kê databases.</summary>
        private string BuildServerConnection()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = cbServer.Text.Trim(),
                TrustServerCertificate = true,
                ConnectTimeout = 5   // timeout ngắn để UX không bị treo lâu
            };

            if (rbWindowsAuth?.IsChecked == true)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = txtUsername.Text.Trim();
                builder.Password = txtPassword.Password;
            }

            return builder.ConnectionString;
        }

        /// <summary>Connection string đầy đủ bao gồm cả database – dùng để lưu và test.</summary>
        private string BuildConnectionString()
        {
            if (rbWindowsAuth == null || txtUsername == null || txtPassword == null)
                return string.Empty;

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = cbServer.Text.Trim(),
                InitialCatalog = cbDatabase.Text.Trim(),
                TrustServerCertificate = true
            };

            if (rbWindowsAuth.IsChecked == true)
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

        #region Load current saved connection

        private void LoadCurrentConnection()
        {
            try
            {
                string conn = ConnectionManager.CurrentConnectionString;
                if (string.IsNullOrWhiteSpace(conn))
                    return;

                var builder = new SqlConnectionStringBuilder(conn);

                // Đảm bảo server đã lưu luôn có mặt trong dropdown (dù máy này không tự detect được)
                if (!cbServer.Items.Contains(builder.DataSource))
                    cbServer.Items.Insert(0, builder.DataSource);

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
                // Bỏ qua lỗi parse – config cũ có thể không hợp lệ
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
            if (cbServer == null || cbDatabase == null || rbSqlAuth == null
                || txtUsername == null || txtPassword == null)
                return;

            bool sqlAuth = rbSqlAuth.IsChecked == true;
            txtUsername.IsEnabled = sqlAuth;
            txtPassword.IsEnabled = sqlAuth;
        }

        #endregion

        #region UI Events

        private void cbServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbServer.SelectedItem == null) return;

            // IsEditable=True nên phải gán Text thủ công sau khi chọn
            cbServer.Text = cbServer.SelectedItem.ToString();
            LoadDatabases();
            txtConnectionString.Text = BuildConnectionString();
        }

        /// <summary>
        /// Khi user gõ tay tên server rồi nhấn Tab / click ra ngoài.
        /// Cần thiết vì SelectionChanged không trigger khi text được nhập thủ công.
        /// </summary>
        private void cbServer_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cbServer.Text))
            {
                LoadDatabases();
                txtConnectionString.Text = BuildConnectionString();
            }
        }

        private void ConnectionInput_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                txtConnectionString.Text = BuildConnectionString();
            }
            catch { }
        }

        #endregion

        #region Test

        private void btnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string conn = BuildConnectionString();
                txtConnectionString.Text = conn;

                using (SqlConnection sql = new SqlConnection(conn))
                    sql.Open();

                SetStatus("✓ Connection successful", isError: false);
                MessageBox.Show("Connection successful.", "Database",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                SetStatus("✗ Connection failed", isError: true);
                MessageBox.Show(ex.Message, "Connection Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

                MessageBox.Show(
                    "Database settings saved successfully.\n\nPlease restart the application.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Close

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Helpers

        private void SetStatus(string message, bool? isError)
        {
            if (message == null)
            {
                txtStatus.Text = string.Empty;
                return;
            }

            txtStatus.Text = message;
            txtStatus.Foreground = isError == true
                ? System.Windows.Media.Brushes.Firebrick
                : System.Windows.Media.Brushes.SeaGreen;
        }

        #endregion
    }
}