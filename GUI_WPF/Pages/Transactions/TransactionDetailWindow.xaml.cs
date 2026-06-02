using ET;
using System;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GUI_WPF.Pages.Transactions
{
    /// <summary>
    /// Interaction logic for TransactionDetailWindow.xaml
    /// </summary>
    public partial class TransactionDetailWindow : Window
    {
         
        // STATE
        private ET_Transactions _transaction;

         
        // CTOR
        public TransactionDetailWindow(ET_Transactions transaction)
        {
            InitializeComponent();

            _transaction = transaction;

            Loaded += TransactionDetailWindow_Loaded;
        }

         
        // LOAD
         

        private void TransactionDetailWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            LoadTransaction();
        }

         
        // LOAD TRANSACTION
         

        private void LoadTransaction()
        {
            if (_transaction == null)
                return;

            txtTransactionTitle.Text =
                $"Transaction #{_transaction.ID}";

            txtAmount.Text =
                $"{_transaction.Amount:N0}$";

            txtDate.Text =
                _transaction.TransactionDate
                .ToString("dd MMM yyyy HH:mm");

            txtGatewayID.Text =
                _transaction.GatewayReturnID;

            txtReference.Text =
                $"REF-{_transaction.ID}";

            // SAFE FALLBACKS
            txtType.Text =
                string.IsNullOrWhiteSpace(
                    _transaction.TransactionTypeName)
                ? "Transaction"
                : _transaction.TransactionTypeName;

            txtStatus.Text =
                string.IsNullOrWhiteSpace(
                    _transaction.StatusName)
                ? "Completed"
                : _transaction.StatusName;

            txtMethod.Text =
                "Online Payment";

            txtBookingName.Text =
                string.IsNullOrWhiteSpace(
                    _transaction.Description)
                ? "Related Booking"
                : _transaction.Description;

            txtBookingDates.Text =
                _transaction.TransactionDate
                .ToString("dd MMM yyyy");

            txtUserName.Text =
                $"User #{_transaction.UserID}";

            txtUserEmail.Text =
                "No email available";

            UpdateStatusUI();

            BuildTimeline();
        }

         
        // STATUS UI
         

        private void UpdateStatusUI()
        {
            switch (txtStatus.Text)
            {
                case "Completed":

                    bdStatus.Background =
                        new SolidColorBrush(
                            Color.FromRgb(233, 255, 242));

                    txtStatus.Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(26, 155, 93));

                    break;

                case "Refunded":

                    bdStatus.Background =
                        new SolidColorBrush(
                            Color.FromRgb(255, 243, 224));

                    txtStatus.Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(214, 122, 0));

                    break;

                case "Failed":

                    bdStatus.Background =
                        new SolidColorBrush(
                            Color.FromRgb(255, 235, 235));

                    txtStatus.Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(214, 69, 69));

                    break;
            }
        }

         
        // TIMELINE
         

        private void BuildTimeline()
        {
            spTimeline.Children.Clear();

            AddTimelineItem(
                "Transaction Created",
                "Payment initialized");

            AddTimelineItem(
                "Gateway Processing",
                "Payment gateway handled request");

            AddTimelineItem(
                txtStatus.Text,
                "Transaction finalized");
        }

        private void AddTimelineItem(
            string title,
            string subtitle)
        {
            Border item =
                new Border
                {
                    Background =
                        Brushes.White,

                    CornerRadius =
                        new CornerRadius(20),

                    Padding =
                        new Thickness(20),

                    Margin =
                        new Thickness(0, 0, 0, 14)
                };

            StackPanel panel =
                new StackPanel();

            TextBlock txtTitle =
                new TextBlock
                {
                    Text = title,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(58, 49, 49))
                };

            TextBlock txtSub =
                new TextBlock
                {
                    Text = subtitle,
                    Margin = new Thickness(0, 8, 0, 0),
                    Foreground = Brushes.Gray
                };

            panel.Children.Add(txtTitle);

            panel.Children.Add(txtSub);

            item.Child = panel;

            spTimeline.Children.Add(item);
        }

         
        // ACTIONS
         

        private void btnRefund_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Refund initiated.",
                "Refund");
        }

        private void btnFlag_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Transaction flagged.",
                "Flag");
        }

        private void btnOpenBooking_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Open booking detail.",
                "Booking");
        }

         
        // EXPORT
         

        private void btnExport_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Export receipt PDF.",
                "Export");
        }

        private void btnPrint_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Print transaction.",
                "Print");
        }

         
        // CLOSE
         

        private void btnClose_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}