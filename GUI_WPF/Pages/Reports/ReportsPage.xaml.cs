using BUS;
using DTO;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System.Diagnostics;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI_WPF.Pages.Reports
{
    public partial class ReportsPage : Page
    {
        private readonly BUS_Report _reportBus =
            new BUS_Report();

        public ReportsPage()
        {
            InitializeComponent();

            Loaded += ReportsPage_Loaded;
        }

        private void ReportsPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            try
            {
                LoadKpiCards();

                LoadRevenueTrend();

                LoadBookingStatus();

                LoadAreaRevenue();

                LoadTopListings();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    ex.Message,
                    "Reports",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        #region KPI

        private void LoadKpiCards()
        {
            DTO_BookingStats stats = _reportBus.GetBookingStats();

            txtTotalRevenue.Text = $"{stats.TotalRevenue:N0} ₫";

            txtTotalBookings.Text = stats.TotalBookings.ToString();

            txtOccupancy.Text = $"{stats.OccupancyRate:N1}%";

            txtAverageBooking.Text = $"{stats.AverageBookingValue:N0} ₫";
        }

        #endregion

        #region Revenue

        private void LoadRevenueTrend()
        {
            dgRevenueTrend.ItemsSource = _reportBus.GetRevenueTrend();
        }

        #endregion

        #region Status

        private void LoadBookingStatus()
        {
            dgBookingStatus.ItemsSource = _reportBus.GetBookingStatus();
        }

        #endregion

        #region Area Revenue

        private void LoadAreaRevenue()
        {
            dgAreaRevenue.ItemsSource = _reportBus.GetRevenueByArea();
        }

        #endregion

        #region Top Listings

        private void LoadTopListings()
        {
            dgTopListings.ItemsSource = _reportBus.GetTopListings();
        }

        #endregion

        public void RefreshData()
        {
            LoadDashboard();
        }
        private void btnExportPdf_Click(
    object sender,
    System.Windows.RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog =
                    new SaveFileDialog();

                dialog.Filter =
                    "PDF Files (*.pdf)|*.pdf";

                dialog.FileName =
                    $"SeoulStay_Report_{DateTime.Now:yyyyMMdd}";

                if (dialog.ShowDialog() != true)
                    return;

                ExportPdf(dialog.FileName);

                System.Windows.MessageBox.Show(
                    "Report exported successfully.",
                    "Export PDF");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    ex.Message,
                    "Export Error");
            }
        }
        private void ExportPdf(string filePath)
        {
            var stats = _reportBus.GetBookingStats();

            var revenueTrend =
                _reportBus.GetRevenueTrend();

            var bookingStatus =
                _reportBus.GetBookingStatus();

            var areaRevenue =
                _reportBus.GetRevenueByArea();

            var topListings =
                _reportBus.GetTopListings();

            Document doc = new Document();

            doc.Styles["Normal"].Font.Name = "Times New Roman";

            doc.Info.Title =
                "Seoul Stay Report";

            doc.Info.Author =
                "Seoul Stay";

            Section section =
                doc.AddSection();

            // =========================
            // HEADER
            // =========================

            Paragraph title =
                section.AddParagraph();

            title.AddFormattedText(
                "SEOUL STAY",
                TextFormat.Bold);

            title.Format.Font.Size = 22;
            title.Format.Alignment =
                ParagraphAlignment.Center;

            Paragraph subTitle =
                section.AddParagraph();

            subTitle.AddText(
                "Rental Housing Management System");

            subTitle.Format.Alignment =
                ParagraphAlignment.Center;

            subTitle.Format.SpaceAfter =
                "0.5cm";

            Paragraph generated =
                section.AddParagraph();

            generated.AddText(
                $"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}");

            generated.Format.Alignment =
                ParagraphAlignment.Center;

            generated.Format.SpaceAfter =
                "0.8cm";

            // =========================
            // DASHBOARD SUMMARY
            // =========================

            Paragraph summaryTitle =
                section.AddParagraph();

            summaryTitle.AddFormattedText(
                "DASHBOARD SUMMARY",
                TextFormat.Bold);

            summaryTitle.Format.Font.Size = 14;

            summaryTitle.Format.SpaceAfter =
                "0.2cm";

            section.AddParagraph(
                $"Total Revenue: {stats.TotalRevenue:N0} ₫");

            section.AddParagraph(
                $"Total Bookings: {stats.TotalBookings}");

            section.AddParagraph(
                $"Occupancy Rate: {stats.OccupancyRate:N1}%");

            section.AddParagraph(
                $"Average Booking Value: {stats.AverageBookingValue:N0} ₫");

            section.AddParagraph(
                $"Today's Revenue: {stats.TodayRevenue:N0} ₫");

            section.AddParagraph(
                $"Monthly Revenue: {stats.MonthlyRevenue:N0} ₫");

            section.AddParagraph(
                $"Active Stays: {stats.ActiveStays}");

            section.AddParagraph(
                $"Returning Guests: {stats.ReturningGuests}");

            section.AddParagraph();

            // =========================
            // REVENUE TREND
            // =========================

            Paragraph revenueTitle =
                section.AddParagraph();

            revenueTitle.AddFormattedText(
                "REVENUE TREND",
                TextFormat.Bold);

            revenueTitle.Format.Font.Size = 14;

            Table revenueTable =
                section.AddTable();

            revenueTable.Borders.Width = 0.75;

            revenueTable.AddColumn("5cm");
            revenueTable.AddColumn("5cm");

            Row header =
                revenueTable.AddRow();

            header.Shading.Color =
                Colors.LightGray;

            header.Cells[0]
                .AddParagraph("Month");

            header.Cells[1]
                .AddParagraph("Revenue");

            foreach (var item in revenueTrend)
            {
                Row row =
                    revenueTable.AddRow();

                row.Cells[0]
                    .AddParagraph(item.Label);

                row.Cells[1]
                    .AddParagraph(
                        item.Revenue.ToString("N0"));
            }

            section.AddParagraph();

            // =========================
            // BOOKING STATUS
            // =========================

            Paragraph statusTitle =
                section.AddParagraph();

            statusTitle.AddFormattedText(
                "BOOKING STATUS",
                TextFormat.Bold);

            statusTitle.Format.Font.Size = 14;

            Table statusTable =
                section.AddTable();

            statusTable.Borders.Width = 0.75;

            statusTable.AddColumn("5cm");
            statusTable.AddColumn("3cm");

            Row h1 =
                statusTable.AddRow();

            h1.Shading.Color =
                Colors.LightGray;

            h1.Cells[0]
                .AddParagraph("Status");

            h1.Cells[1]
                .AddParagraph("Count");

            foreach (var item in bookingStatus)
            {
                Row row =
                    statusTable.AddRow();

                row.Cells[0]
                    .AddParagraph(item.Status);

                row.Cells[1]
                    .AddParagraph(item.Count.ToString());
            }

            section.AddParagraph();

            // =========================
            // REVENUE BY AREA
            // =========================

            Paragraph areaTitle =
                section.AddParagraph();

            areaTitle.AddFormattedText(
                "REVENUE BY AREA",
                TextFormat.Bold);

            areaTitle.Format.Font.Size = 14;

            Table areaTable =
                section.AddTable();

            areaTable.Borders.Width = 0.75;

            areaTable.AddColumn("5cm");
            areaTable.AddColumn("5cm");

            Row h2 =
                areaTable.AddRow();

            h2.Shading.Color =
                Colors.LightGray;

            h2.Cells[0]
                .AddParagraph("Area");

            h2.Cells[1]
                .AddParagraph("Revenue");

            foreach (var item in areaRevenue)
            {
                Row row =
                    areaTable.AddRow();

                row.Cells[0]
                    .AddParagraph(item.AreaName);

                row.Cells[1]
                    .AddParagraph(
                        item.Revenue.ToString("N0"));
            }

            section.AddParagraph();

            // =========================
            // TOP LISTINGS
            // =========================

            Paragraph listingTitle =
                section.AddParagraph();

            listingTitle.AddFormattedText(
                "TOP LISTINGS",
                TextFormat.Bold);

            listingTitle.Format.Font.Size = 14;

            Table listingTable =
                section.AddTable();

            listingTable.Borders.Width = 0.75;

            listingTable.AddColumn("8cm");
            listingTable.AddColumn("3cm");
            listingTable.AddColumn("3cm");

            Row h3 =
                listingTable.AddRow();

            h3.Shading.Color =
                Colors.LightGray;

            h3.Cells[0]
                .AddParagraph("Listing");

            h3.Cells[1]
                .AddParagraph("Revenue");

            h3.Cells[2]
                .AddParagraph("Bookings");

            foreach (var item in topListings)
            {
                Row row =
                    listingTable.AddRow();

                row.Cells[0]
                    .AddParagraph(item.Title);

                row.Cells[1]
                    .AddParagraph(
                        item.Revenue.ToString("N0"));

                row.Cells[2]
                    .AddParagraph(
                        item.BookingCount.ToString());
            }

            PdfDocumentRenderer renderer =
                new PdfDocumentRenderer(true);

            renderer.Document = doc;

            renderer.RenderDocument();

            renderer.PdfDocument.Save(filePath);

            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }
    }
}