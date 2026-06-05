using GUI_WPF.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GUI_WPF.Pages.Bookings
{
    /// <summary>
    /// Dialog hiển thị sau khi booking + invoice tạo thành công.
    /// Hiện QR code, PDF path, và lắng nghe SignalR để nhận kết quả thanh toán.
    /// </summary>
    public partial class InvoiceGeneratedDialog : Window
    {
        private readonly long _invoiceId;
        private readonly string _pdfPath;
        private bool _paymentReceived = false;

        // =============================================
        // CONSTRUCTOR
        // =============================================

        /// <param name="qrImage">BitmapImage QR code (từ Helper_QRCode.GenerateBitmapImage)</param>
        /// <param name="pdfPath">Đường dẫn file PDF đã export</param>
        /// <param name="invoiceId">ID của invoice (để filter SignalR event)</param>
        /// <param name="invoiceCode">Mã invoice hiển thị (vd: INV-000042)</param>
        /// <param name="totalAmount">Tổng tiền hiển thị</param>
        public InvoiceGeneratedDialog(
            BitmapImage qrImage,
            string pdfPath,
            long invoiceId,
            string invoiceCode,
            string totalAmountDisplay)
        {
            InitializeComponent();

            _invoiceId = invoiceId;
            _pdfPath = pdfPath;

            // Set UI
            imgQrCode.Source = qrImage;
            txtPdfPath.Text = pdfPath;
            txtInvoiceCode.Text = invoiceCode;
            txtTotalAmount.Text = totalAmountDisplay;

            // Draggable window (WindowStyle = None)
            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            };

            // Subscribe SignalR event
            SignalRService.Instance.OnPaymentResult += OnPaymentResultReceived;

            // Start spinner animation
            Loaded += (s, e) =>
            {
                var storyboard = (Storyboard)FindResource("SpinAnimation");
                storyboard.Begin();
            };
        }

        // =============================================
        // SIGNALR EVENT HANDLER
        // =============================================

        private void OnPaymentResultReceived(long invoiceId, string status)
        {
            // Chỉ xử lý event cho đúng invoice này
            if (invoiceId != _invoiceId)
                return;

            _paymentReceived = true;

            // Stop spinner
            var storyboard = (Storyboard)FindResource("SpinAnimation");
            storyboard.Stop();

            // Ẩn waiting panel
            WaitingPanel.Visibility = Visibility.Collapsed;

            if (status == "Paid")
            {
                CompletedPanel.Visibility = Visibility.Visible;

                // Hiện PaymentResultWindow
                var resultWindow = new PaymentResultWindow(true);
                resultWindow.Owner = this;
                resultWindow.ShowDialog();
            }
            else
            {
                // Cancelled hoặc status khác
                CancelledPanel.Visibility = Visibility.Visible;

                var resultWindow = new PaymentResultWindow(false);
                resultWindow.Owner = this;
                resultWindow.ShowDialog();
            }
        }

        // =============================================
        // BUTTON HANDLERS
        // =============================================

        private void btnOpenPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_pdfPath) && System.IO.File.Exists(_pdfPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = _pdfPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show(
                        "PDF file not found at the specified path.",
                        "File Not Found",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to open PDF: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // Unsubscribe event trước khi close
            SignalRService.Instance.OnPaymentResult -= OnPaymentResultReceived;

            // Disconnect SignalR nếu cần
            try
            {
                await SignalRService.Instance.DisconnectAsync();
            }
            catch { }

            DialogResult = _paymentReceived;
            Close();
        }

        // =============================================
        // CLEANUP khi window bị đóng (X button hoặc Alt+F4)
        // =============================================

        protected override async void OnClosed(EventArgs e)
        {
            SignalRService.Instance.OnPaymentResult -= OnPaymentResultReceived;

            try
            {
                await SignalRService.Instance.DisconnectAsync();
            }
            catch { }

            base.OnClosed(e);
        }
    }
}