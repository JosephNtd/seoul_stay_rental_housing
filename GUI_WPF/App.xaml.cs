using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace GUI_WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Tự động kích hoạt Server Web Payment chạy ngầm
            StartWebPaymentServer();
        }

        private void StartWebPaymentServer()
        {
            try
            {
                // 1. Xác định thư mục chứa mã nguồn Web nằm chung cấp với file .exe của WPF
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string webDir = Path.Combine(appDir, "GUI_Web_Payment");

                // 2. Tìm đường dẫn file chạy IIS Express mặc định của Windows
                string iisExpressPath = Environment.Is64BitOperatingSystem
                    ? @"C:\Program Files\IIS Express\iisexpress.exe"
                    : @"C:\Program Files (x86)\IIS Express\iisexpress.exe";

                // 3. Nếu máy có cài IIS Express và tìm thấy thư mục Web thì kích hoạt chạy ngầm
                if (File.Exists(iisExpressPath) && Directory.Exists(webDir))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = iisExpressPath,
                        // Tham số ép IIS Express chạy thư mục Web của bro đúng cổng 5000
                        Arguments = $"/path:\"{webDir}\" /port:5000",
                        CreateNoWindow = true,       // Ẩn cửa sổ đen của IIS Express đi cho sạch
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Không thể khởi động Web Server: {ex.Message}");
            }
        }
    }
}