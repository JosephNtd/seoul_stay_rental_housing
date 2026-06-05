using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace GUI_WPF.Pages.Bookings
{
    /// <summary>
    /// Cửa sổ hiển thị kết quả thanh toán: "Payment Successful" hoặc "Payment Cancelled".
    /// Được gọi từ InvoiceGeneratedDialog khi nhận được SignalR event.
    /// </summary>
    public partial class PaymentResultWindow : Window
    {
        // =============================================
        // CONSTRUCTOR
        // =============================================

        /// <param name="isSuccess">true = Paid, false = Cancelled</param>
        public PaymentResultWindow(bool isSuccess)
        {
            InitializeComponent();

            if (isSuccess)
            {
                ApplySuccessTheme();
            }
            else
            {
                ApplyCancelledTheme();
            }

            // Draggable
            MouseLeftButtonDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            };

            // Play animation
            Loaded += (s, e) =>
            {
                var storyboard = (Storyboard)FindResource("BounceIn");
                storyboard.Begin();
            };
        }

        // =============================================
        // THEME
        // =============================================

        private void ApplySuccessTheme()
        {
            txtIcon.Text = "✓";
            txtTitle.Text = "Payment Successful";
            txtMessage.Text = "The guest has completed the QR payment.\nThe booking is now confirmed.";

            // Green gradient icon
            IconGrad1.Color = (Color)ColorConverter.ConvertFromString("#4CAF50");
            IconGrad2.Color = (Color)ColorConverter.ConvertFromString("#2E7D32");

            // Green shadow
            ((DropShadowEffect)SuccessIcon.Effect).Color =
                (Color)ColorConverter.ConvertFromString("#4CAF50");

            // Green button style (SuccessButtonStyle already defined)
            btnOk.Style = (Style)FindResource("SuccessButtonStyle");
        }

        private void ApplyCancelledTheme()
        {
            txtIcon.Text = "✕";
            txtTitle.Text = "Payment Cancelled";
            txtMessage.Text = "The guest has cancelled the payment.\nThe booking remains in Pending status.";

            // Red gradient icon
            IconGrad1.Color = (Color)ColorConverter.ConvertFromString("#E57373");
            IconGrad2.Color = (Color)ColorConverter.ConvertFromString("#D32F2F");

            // Red shadow
            ((DropShadowEffect)SuccessIcon.Effect).Color =
                (Color)ColorConverter.ConvertFromString("#D32F2F");

            // Red button
            btnOk.Style = (Style)FindResource("DangerButtonStyle");
        }

        // =============================================
        // BUTTON
        // =============================================

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}