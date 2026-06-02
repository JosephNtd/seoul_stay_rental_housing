using DTO;
using System;
using System.Windows.Controls;

namespace GUI_WPF.UserControls.Panels
{
    public partial class BookingStatsPanel : UserControl
    {
        public BookingStatsPanel()
        {
            InitializeComponent();

            LastUpdated = $"Updated {DateTime.Now:HH:mm}";
        }

        public string LastUpdated { get; set; }

        public DTO_BookingStats Stats { get; private set; }

        public void LoadStats(DTO_BookingStats stats)
        {
            if (stats == null)
                return;

            Stats = stats;

            txtTotalBookings.Text = stats.TotalBookings.ToString();
            txtPending.Text = stats.PendingBookings.ToString();
            txtTodayCheckIn.Text = stats.TodayCheckIns.ToString();
            txtTodayCheckOut.Text = stats.TodayCheckOuts.ToString();
            txtActiveStays.Text = stats.ActiveStays.ToString();
            txtRevenue.Text = $"${stats.TotalRevenue:N0}";

            LastUpdated = $"Updated {DateTime.Now:HH:mm}";
        }
    }
}