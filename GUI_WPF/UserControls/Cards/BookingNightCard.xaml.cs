using DTO;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI_WPF.UserControls.Cards
{
    public partial class BookingNightCard : UserControl
    {
        public BookingNightCard()
        {
            InitializeComponent();
        }

        public DTO_BookingNight Night { get; private set; }

        public void LoadNight(DTO_BookingNight night)
        {
            if (night == null)
                return;

            Night = night;

            txtDay.Text = night.Date.Day.ToString();

            txtMonth.Text = night.Date.ToString("MMM").ToUpper();

            txtDate.Text = night.Date.ToString("dd MMM yyyy");

            txtPrice.Text = $"${night.BasePrice:N0}";

            txtPolicy.Text = string.IsNullOrWhiteSpace(night.RefundPolicyName) ? "No Refund Policy" : night.RefundPolicyName;

            LoadRefundStatus();
        }
        public void LoadNight(DTO_CreateBookingNight night)
        {
            txtDate.Text = night.DateDisplay;

            txtPrice.Text = night.PriceDisplay;

            txtPolicy.Text = night.PolicyName;
        }

        private void LoadRefundStatus()
        {
            if (Night.IsRefund)
            {
                RefundBadge.Background = Brushes.MistyRose;
                txtRefundStatus.Text = "Refunded";
                txtRefundStatus.Foreground = Brushes.Firebrick;
            }
            else
            {
                RefundBadge.Background = Brushes.Honeydew;
                txtRefundStatus.Text = "Active";
                txtRefundStatus.Foreground = Brushes.SeaGreen;
            }
        }
    }
}