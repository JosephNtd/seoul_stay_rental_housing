using BUS;
using DTO;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WPF;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI_WPF.Pages.Reports
{
    public partial class ReportsPage : Page
    {
        private readonly BUS_Report _reportBus =
            new BUS_Report();

        // ══════════════════════════════════════
        //  CACHED DATA (dùng lại khi export)
        // ══════════════════════════════════════

        private DTO_BookingStats _stats;

        private List<DTO_RevenueTrend> _revenueTrend;

        private List<DTO_BookingStatusReport> _bookingStatus;

        private List<DTO_AreaRevenueReport> _areaRevenue;

        private List<DTO_TopListingReport> _topListings;

        // ══════════════════════════════════════
        //  STATUS → COLOR MAP
        // ══════════════════════════════════════

        private static readonly Dictionary<string, string> StatusColors =
            new Dictionary<string, string>
            {
                { "Pending",   "#F4A261" },
                { "Confirmed", "#2AA876" },
                { "CheckedIn", "#3A86E9" },
                { "Completed", "#6C63FF" },
                { "Cancelled", "#E24B4A" },
                { "Refunded",  "#888780" }
            };

        // ══════════════════════════════════════
        //  CHART THEME COLORS
        // ══════════════════════════════════════

        private static readonly SKColor Blue = SKColor.Parse("#3A86E9");
        private static readonly SKColor Green = SKColor.Parse("#2AA876");
        private static readonly SKColor Purple = SKColor.Parse("#6C63FF");
        private static readonly SKColor Amber = SKColor.Parse("#F4A261");
        private static readonly SKColor TextDark = SKColor.Parse("#3A3131");
        private static readonly SKColor TextMuted = SKColor.Parse("#8B7D7D");
        private static readonly SKColor GridLine = SKColor.Parse("#F0DEDE");

        // ══════════════════════════════════════
        //  CONSTRUCTOR
        // ══════════════════════════════════════

        public ReportsPage()
        {
            InitializeComponent();

            Loaded += ReportsPage_Loaded;
        }

        private void ReportsPage_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            LoadDashboard();
        }

        // ══════════════════════════════════════════════
        //  LOAD DASHBOARD (master)
        // ══════════════════════════════════════════════

        private void LoadDashboard()
        {
            try
            {
                // Load data một lần, cache lại
                _stats = _reportBus.GetBookingStats();
                _revenueTrend = _reportBus.GetRevenueTrend();
                _bookingStatus = _reportBus.GetBookingStatus();
                _areaRevenue = _reportBus.GetRevenueByArea();
                _topListings = _reportBus.GetTopListings();

                MessageBox.Show(
                                $"RevenueTrend={_revenueTrend?.Count}\n" +
                                $"BookingStatus={_bookingStatus?.Count}\n" +
                                $"AreaRevenue={_areaRevenue?.Count}\n" +
                                $"TopListings={_topListings?.Count}");

                LoadKpiCards();
                LoadRevenueTrendChart();
                LoadBookingStatusChart();
                LoadAreaRevenueChart();
                LoadTopListingsChart();
                LoadDataTables();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Reports — Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #region ══ KPI CARDS ══

        private void LoadKpiCards()
        {
            // Row 1 — Primary
            txtTotalRevenue.Text = $"{_stats.TotalRevenue:N0} ₫";
            txtTotalBookings.Text = _stats.TotalBookings.ToString("N0");
            txtOccupancy.Text = $"{_stats.OccupancyRate:N1}%";
            txtOccupancyLevel.Text = _stats.OccupancyLevel;
            txtAverageBooking.Text = $"{_stats.AverageBookingValue:N0} ₫";

            // Row 2 — Secondary
            txtTodayRevenue.Text = $"{_stats.TodayRevenue:N0} ₫";
            txtMonthlyRevenue.Text = $"{_stats.MonthlyRevenue:N0} ₫";
            txtActiveStays.Text = _stats.ActiveStays.ToString();
            txtReturningGuests.Text = _stats.ReturningGuests.ToString();
        }

        #endregion

        #region ══ REVENUE TREND — LINE + AREA ══

        private void LoadRevenueTrendChart()
        {
            if (_revenueTrend == null || !_revenueTrend.Any())
                return;

            // ************************
            var first = _revenueTrend.First();
            Debug.WriteLine($"RevenueTrend: Label={first.Label}, Revenue={first.Revenue}");
            // ************************

            chartRevenueTrend.Series = new ISeries[]
            {
                new LineSeries<decimal>
                {
                    Values = _revenueTrend
                        .Select(t => t.Revenue)
                        .ToArray(),

                    Name = "Revenue",

                    // Line stroke
                    Stroke = new SolidColorPaint(Blue)
                    {
                        StrokeThickness = 3
                    },

                    // Area fill (transparent)
                    Fill = new SolidColorPaint(
                        Blue.WithAlpha(35)),

                    // Data point geometry
                    GeometrySize   = 8,
                    GeometryStroke = new SolidColorPaint(Blue)
                    {
                        StrokeThickness = 2
                    },
                    GeometryFill = new SolidColorPaint(SKColors.White),

                    // Tooltip
                    YToolTipLabelFormatter = p =>
                        $"{p.Model:N0} ₫",

                    LineSmoothness = 0.3
                }
            };

            chartRevenueTrend.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = _revenueTrend
                        .Select(t => t.Label)
                        .ToArray(),

                    LabelsPaint = new SolidColorPaint(TextMuted),
                    TextSize    = 12,

                    SeparatorsPaint = new SolidColorPaint(GridLine)
                    {
                        StrokeThickness = 0.5f
                    },

                    LabelsRotation = _revenueTrend.Count > 8
                        ? 45
                        : 0
                }
            };

            chartRevenueTrend.YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler     = v => $"{v:N0}",
                    LabelsPaint = new SolidColorPaint(TextMuted),
                    TextSize    = 11,

                    SeparatorsPaint = new SolidColorPaint(GridLine)
                    {
                        StrokeThickness = 0.5f
                    }
                }
            };

            chartRevenueTrend.UpdateLayout();   // Force immediate update
            chartRevenueTrend.InvalidateVisual();
            chartRevenueTrend.LegendPosition = LegendPosition.Hidden;
        }

        #endregion

        #region ══ BOOKING STATUS — DONUT ══

        private void LoadBookingStatusChart()
        {
            if (_bookingStatus == null || !_bookingStatus.Any())
                return;

            var series = new List<ISeries>();
            var legends = new List<LegendItem>();

            int total = _bookingStatus.Sum(s => s.Count);

            foreach (var item in _bookingStatus)
            {
                string hex = StatusColors.ContainsKey(item.Status)
                    ? StatusColors[item.Status]
                    : "#B9A9A9";

                SKColor color = SKColor.Parse(hex);

                double pct = total > 0
                    ? item.Count * 100.0 / total
                    : 0;

                series.Add(new PieSeries<int>
                {
                    Values = new[] { item.Count },
                    Name = $"{item.Status}: {item.Count} ({pct:N1}%)",
                    InnerRadius = 70,

                    Fill = new SolidColorPaint(color)
                });

                legends.Add(new LegendItem
                {
                    Color = new SolidColorBrush(
                        (System.Windows.Media.Color)
                            ColorConverter.ConvertFromString(hex)),

                    Label = $"{item.Status} ({item.Count})"
                });
            }

            chartBookingStatus.Series = series;

            chartBookingStatus.LegendPosition = LegendPosition.Hidden;

            // Bind custom legend
            icStatusLegend.ItemsSource = legends;
        }

        #endregion

        #region ══ AREA REVENUE — HORIZONTAL BAR ══

        private void LoadAreaRevenueChart()
        {
            if (_areaRevenue == null || !_areaRevenue.Any())
                return;
            // ************************
            var first = _areaRevenue.First();
            Debug.WriteLine($"RevenueArea: Label={first.AreaName}, Revenue={first.Revenue}");
            // ************************
            // Reverse vì RowSeries hiển thị bottom → top
            var reversed = _areaRevenue
                .AsEnumerable()
                .Reverse()
                .ToList();

            chartAreaRevenue.Series = new ISeries[]
            {
                new RowSeries<decimal>
                {
                    Values = reversed
                        .Select(a => a.Revenue)
                        .ToArray(),

                    Name = "Revenue",

                    Fill = new SolidColorPaint(Green),

                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize  = 11,
                    DataLabelsPosition = DataLabelsPosition.Middle,
                    DataLabelsFormatter = p => $"{p.Model:N0} ₫",

                    MaxBarWidth = 28,

                    YToolTipLabelFormatter = p =>
                        $"{p.Model:N0} ₫"
                }
            };

            chartAreaRevenue.YAxes = new Axis[]
            {
                new Axis
                {
                    Labels = reversed
                        .Select(a => a.AreaName)
                        .ToArray(),

                    LabelsPaint = new SolidColorPaint(TextDark),
                    TextSize    = 12,

                    SeparatorsPaint = new SolidColorPaint(
                        SKColors.Transparent)
                }
            };

            chartAreaRevenue.XAxes = new Axis[]
            {
                new Axis
                {
                    Labeler     = v => $"{v:N0}",
                    MinLimit = 0,
                    LabelsPaint = new SolidColorPaint(TextMuted),
                    TextSize    = 10,

                    SeparatorsPaint = new SolidColorPaint(GridLine)
                    {
                        StrokeThickness = 0.5f
                    }
                }
            };
            chartAreaRevenue.UpdateLayout();   // Force immediate update
            chartAreaRevenue.InvalidateVisual();
            chartAreaRevenue.LegendPosition = LegendPosition.Hidden;
        }

        #endregion

        #region ══ TOP LISTINGS — COLUMN + LINE COMBO ══

        private void LoadTopListingsChart()
        {
            if (_topListings == null || !_topListings.Any())
                return;
            // ************************
            var first = _topListings.First();
            Debug.WriteLine($"TopListing: Label={first.Title}, Revenue={first.Revenue}");
            // ************************
            chartTopListings.Series = new ISeries[]
            {
                // Column — Revenue (left Y axis)
                new ColumnSeries<decimal>
                {
                    Values = _topListings
                        .Select(l => l.Revenue)
                        .ToArray(),

                    Name = "Revenue",

                    Fill = new SolidColorPaint(Purple),

                    MaxBarWidth = 36,

                    ScalesYAt = 0,

                    YToolTipLabelFormatter = p =>
                        $"Revenue: {p.Model:N0} ₫"
                },

                // Line — Booking count (right Y axis)
                new LineSeries<int>
                {
                    Values = _topListings
                        .Select(l => l.BookingCount)
                        .ToArray(),

                    Name = "Bookings",

                    Stroke = new SolidColorPaint(Amber)
                    {
                        StrokeThickness = 3
                    },

                    Fill = null,

                    GeometrySize   = 8,
                    GeometryStroke = new SolidColorPaint(Amber)
                    {
                        StrokeThickness = 2
                    },
                    GeometryFill = new SolidColorPaint(SKColors.White),

                    ScalesYAt = 1,

                    LineSmoothness = 0.35,

                    YToolTipLabelFormatter = p =>
                        $"Bookings: {p.Model}"
                }
            };

            // X Axis — Listing titles (truncated)
            chartTopListings.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = _topListings
                        .Select(l => l.Title.Length > 18
                            ? l.Title.Substring(0, 18) + "…"
                            : l.Title)
                        .ToArray(),

                    LabelsPaint    = new SolidColorPaint(TextMuted),
                    TextSize       = 11,
                    LabelsRotation = 25,

                    SeparatorsPaint = new SolidColorPaint(
                        SKColors.Transparent)
                }
            };

            // Dual Y Axes
            chartTopListings.YAxes = new Axis[]
            {
                // Left — Revenue
                new Axis
                {
                    Name        = "Revenue (₫)",
                    NamePaint   = new SolidColorPaint(Purple),
                    NameTextSize = 12,

                    Labeler     = v => $"{v:N0}",
                    LabelsPaint = new SolidColorPaint(TextMuted),
                    TextSize    = 10,

                    Position = AxisPosition.Start,

                    SeparatorsPaint = new SolidColorPaint(GridLine)
                    {
                        StrokeThickness = 0.5f
                    }
                },

                // Right — Bookings
                new Axis
                {
                    Name        = "Bookings",
                    NamePaint   = new SolidColorPaint(Amber),
                    NameTextSize = 12,

                    Labeler     = v => $"{v:N0}",
                    LabelsPaint = new SolidColorPaint(TextMuted),
                    TextSize    = 10,

                    Position    = AxisPosition.End,

                    SeparatorsPaint = new SolidColorPaint(
                        SKColors.Transparent),

                    ShowSeparatorLines = false
                }
            };
            chartTopListings.UpdateLayout();   // Force immediate update
            chartTopListings.InvalidateVisual();
            chartTopListings.LegendPosition = LegendPosition.Hidden;

            
        }

        #endregion

        #region ══ DATA TABLES ══

        private void LoadDataTables()
        {
            dgRevenueTrend.ItemsSource = _revenueTrend;
            dgBookingStatus.ItemsSource = _bookingStatus;
            dgAreaRevenue.ItemsSource = _areaRevenue;
            dgTopListings.ItemsSource = _topListings;
        }

        #endregion

        #region ══ REFRESH ══

        public void RefreshData()
        {
            LoadDashboard();
        }

        private void btnRefresh_Click(
            object sender,
            RoutedEventArgs e)
        {
            RefreshData();
        }

        #endregion

        // ═════════════════════════════════════════════════
        //
        //   PDF EXPORT
        //
        // ═════════════════════════════════════════════════

        #region ══ EXPORT — FULL REPORT ══

        private void btnExportPdf_Click(
            object sender,
            RoutedEventArgs e)
        {
            string path = ShowSaveDialog(
                $"SeoulStay_FullReport_{DateTime.Now:yyyyMMdd}");

            if (path == null) return;

            try
            {
                ExportFullReport(path);

                MessageBox.Show(
                    "Full report exported successfully!",
                    "Export PDF",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Export Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExportFullReport(string filePath)
        {
            Document doc = CreatePdfDocument();
            Section sec = doc.AddSection();

            AddPdfHeader(sec, "FULL ANALYTICS REPORT");
            AddPdfKpiSummary(sec);
            AddPdfRevenueTrendSection(sec);
            AddPdfBookingStatusSection(sec);
            AddPdfAreaRevenueSection(sec);
            AddPdfTopListingsSection(sec);

            RenderAndOpenPdf(doc, filePath);
        }

        #endregion

        #region ══ EXPORT — REVENUE TREND ══

        private void btnExportRevenueTrend_Click(
            object sender,
            RoutedEventArgs e)
        {
            string path = ShowSaveDialog(
                $"SeoulStay_RevenueTrend_{DateTime.Now:yyyyMMdd}");

            if (path == null) return;

            try
            {
                Document doc = CreatePdfDocument();
                Section sec = doc.AddSection();

                AddPdfHeader(sec, "REVENUE TREND REPORT");
                AddPdfKpiSummary(sec);

                // Chart image
                AddChartImage(sec, chartRevenueTrend);

                AddPdfRevenueTrendSection(sec);

                RenderAndOpenPdf(doc, path);

                ShowExportSuccess();
            }
            catch (Exception ex)
            {
                ShowExportError(ex);
            }
        }

        #endregion

        #region ══ EXPORT — BOOKING STATUS ══

        private void btnExportBookingStatus_Click(
            object sender,
            RoutedEventArgs e)
        {
            string path = ShowSaveDialog(
                $"SeoulStay_BookingStatus_{DateTime.Now:yyyyMMdd}");

            if (path == null) return;

            try
            {
                Document doc = CreatePdfDocument();
                Section sec = doc.AddSection();

                AddPdfHeader(sec, "BOOKING STATUS REPORT");

                // Chart image
                AddChartImage(sec, chartBookingStatus);

                AddPdfBookingStatusSection(sec);

                RenderAndOpenPdf(doc, path);

                ShowExportSuccess();
            }
            catch (Exception ex)
            {
                ShowExportError(ex);
            }
        }

        #endregion

        #region ══ EXPORT — AREA REVENUE ══

        private void btnExportAreaRevenue_Click(
            object sender,
            RoutedEventArgs e)
        {
            string path = ShowSaveDialog(
                $"SeoulStay_AreaRevenue_{DateTime.Now:yyyyMMdd}");

            if (path == null) return;

            try
            {
                Document doc = CreatePdfDocument();
                Section sec = doc.AddSection();

                AddPdfHeader(sec, "REVENUE BY AREA REPORT");

                // Chart image
                AddChartImage(sec, chartAreaRevenue);

                AddPdfAreaRevenueSection(sec);

                RenderAndOpenPdf(doc, path);

                ShowExportSuccess();
            }
            catch (Exception ex)
            {
                ShowExportError(ex);
            }
        }

        #endregion

        #region ══ EXPORT — TOP LISTINGS ══

        private void btnExportTopListings_Click(
            object sender,
            RoutedEventArgs e)
        {
            string path = ShowSaveDialog(
                $"SeoulStay_TopListings_{DateTime.Now:yyyyMMdd}");

            if (path == null) return;

            try
            {
                Document doc = CreatePdfDocument();
                Section sec = doc.AddSection();

                AddPdfHeader(sec, "TOP LISTINGS REPORT");

                // Chart image
                AddChartImage(sec, chartTopListings);

                AddPdfTopListingsSection(sec);

                RenderAndOpenPdf(doc, path);

                ShowExportSuccess();
            }
            catch (Exception ex)
            {
                ShowExportError(ex);
            }
        }

        #endregion

        #region ══ EXPORT — TABLE ONLY ══

        private void btnExportTablePdf_Click(
            object sender,
            RoutedEventArgs e)
        {
            string path = ShowSaveDialog(
                $"SeoulStay_ListingDetails_{DateTime.Now:yyyyMMdd}");

            if (path == null) return;

            try
            {
                Document doc = CreatePdfDocument();
                Section sec = doc.AddSection();

                AddPdfHeader(sec, "LISTING DETAILS REPORT");
                AddPdfKpiSummary(sec);
                AddPdfTopListingsSection(sec);

                RenderAndOpenPdf(doc, path);

                ShowExportSuccess();
            }
            catch (Exception ex)
            {
                ShowExportError(ex);
            }
        }

        #endregion

        // ═════════════════════════════════════════════════
        //
        //   PDF BUILDER HELPERS
        //
        // ═════════════════════════════════════════════════

        #region ══ PDF HELPERS ══

        /// <summary>
        /// Tạo Document cơ bản với font mặc định.
        /// </summary>
        private Document CreatePdfDocument()
        {
            Document doc = new Document();

            doc.Styles["Normal"].Font.Name = "Times New Roman";

            doc.Info.Title = "Seoul Stay Report";
            doc.Info.Author = "Seoul Stay System";

            return doc;
        }

        /// <summary>
        /// Header chung: Logo text + subtitle + ngày giờ.
        /// </summary>
        private void AddPdfHeader(Section section, string reportTitle)
        {
            // LOGO
            Paragraph logo = section.AddParagraph();
            logo.AddFormattedText("SEOUL STAY", TextFormat.Bold);
            logo.Format.Font.Size = 24;
            logo.Format.Font.Color = new MigraDoc.DocumentObjectModel.Color(216, 156, 157);
            logo.Format.Alignment = ParagraphAlignment.Center;

            // SYSTEM SUBTITLE
            Paragraph system = section.AddParagraph();
            system.AddText("Rental Housing Management System");
            system.Format.Font.Size = 11;
            system.Format.Font.Color = new MigraDoc.DocumentObjectModel.Color(139, 125, 125);
            system.Format.Alignment = ParagraphAlignment.Center;
            system.Format.SpaceAfter = "0.3cm";

            // REPORT TITLE
            Paragraph title = section.AddParagraph();
            title.AddFormattedText(reportTitle, TextFormat.Bold);
            title.Format.Font.Size = 16;
            title.Format.Alignment = ParagraphAlignment.Center;
            title.Format.SpaceAfter = "0.2cm";

            // GENERATED DATE
            Paragraph date = section.AddParagraph();
            date.AddText($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}");
            date.Format.Font.Size = 10;
            date.Format.Font.Color = new MigraDoc.DocumentObjectModel.Color(185, 169, 169);
            date.Format.Alignment = ParagraphAlignment.Center;
            date.Format.SpaceAfter = "0.8cm";

            // SEPARATOR LINE
            Paragraph sep = section.AddParagraph();
            sep.Format.Borders.Bottom.Width = 0.5;
            sep.Format.Borders.Bottom.Color = new MigraDoc.DocumentObjectModel.Color(240, 222, 222);
            sep.Format.SpaceAfter = "0.5cm";
        }

        /// <summary>
        /// Dashboard KPI summary block.
        /// </summary>
        private void AddPdfKpiSummary(Section section)
        {
            if (_stats == null) return;

            Paragraph heading = section.AddParagraph();
            heading.AddFormattedText("DASHBOARD SUMMARY", TextFormat.Bold);
            heading.Format.Font.Size = 14;
            heading.Format.SpaceAfter = "0.3cm";

            // KPI table 2 cột
            Table kpi = section.AddTable();
            kpi.Borders.Width = 0;
            kpi.AddColumn("8cm");
            kpi.AddColumn("8cm");

            AddKpiRow(kpi, "Total Revenue", $"{_stats.TotalRevenue:N0} ₫",
                           "Total Bookings", _stats.TotalBookings.ToString());

            AddKpiRow(kpi, "Occupancy Rate", $"{_stats.OccupancyRate:N1}%",
                           "Average Booking", $"{_stats.AverageBookingValue:N0} ₫");

            AddKpiRow(kpi, "Today's Revenue", $"{_stats.TodayRevenue:N0} ₫",
                           "Monthly Revenue", $"{_stats.MonthlyRevenue:N0} ₫");

            AddKpiRow(kpi, "Active Stays", _stats.ActiveStays.ToString(),
                           "Returning Guests", _stats.ReturningGuests.ToString());

            AddKpiRow(kpi, "Avg Stay Length", $"{_stats.AverageStayLength:N1} nights",
                           "Avg Night Price", $"{_stats.AverageNightPrice:N0} ₫");

            section.AddParagraph().Format.SpaceAfter = "0.5cm";
        }

        private void AddKpiRow(
            Table table,
            string label1, string value1,
            string label2, string value2)
        {
            Row row = table.AddRow();
            row.Height = "0.7cm";

            Paragraph p1 = row.Cells[0].AddParagraph();
            p1.AddFormattedText($"{label1}: ", TextFormat.Bold);
            p1.AddText(value1);
            p1.Format.Font.Size = 10;

            Paragraph p2 = row.Cells[1].AddParagraph();
            p2.AddFormattedText($"{label2}: ", TextFormat.Bold);
            p2.AddText(value2);
            p2.Format.Font.Size = 10;
        }

        /// <summary>
        /// Revenue Trend data table.
        /// </summary>
        private void AddPdfRevenueTrendSection(Section section)
        {
            if (_revenueTrend == null || !_revenueTrend.Any())
                return;

            AddSectionTitle(section, "REVENUE TREND");

            Table table = CreateStyledTable(section,
                ("Month", "6cm"),
                ("Revenue (₫)", "6cm"));

            foreach (var item in _revenueTrend)
            {
                Row row = table.AddRow();
                row.Cells[0].AddParagraph(item.Label);
                row.Cells[1].AddParagraph($"{item.Revenue:N0}");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Right;
            }

            // Total row
            Row total = table.AddRow();
            total.Shading.Color = new MigraDoc.DocumentObjectModel.Color(255, 240, 240);
            total.Cells[0].AddParagraph("TOTAL")
                .Format.Font.Bold = true;
            total.Cells[1].AddParagraph($"{_revenueTrend.Sum(r => r.Revenue):N0}")
                .Format.Font.Bold = true;
            total.Cells[1].Format.Alignment = ParagraphAlignment.Right;

            section.AddParagraph().Format.SpaceAfter = "0.5cm";
        }

        /// <summary>
        /// Booking Status data table.
        /// </summary>
        private void AddPdfBookingStatusSection(Section section)
        {
            if (_bookingStatus == null || !_bookingStatus.Any())
                return;

            AddSectionTitle(section, "BOOKING STATUS");

            int total = _bookingStatus.Sum(s => s.Count);

            Table table = CreateStyledTable(section,
                ("Status", "5cm"),
                ("Count", "3cm"),
                ("(%)", "3cm"));

            foreach (var item in _bookingStatus)
            {
                double pct = total > 0
                    ? item.Count * 100.0 / total
                    : 0;

                Row row = table.AddRow();
                row.Cells[0].AddParagraph(item.Status);
                row.Cells[1].AddParagraph(item.Count.ToString());
                row.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[2].AddParagraph($"{pct:N1}%");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Right;
            }

            // Total
            Row totalRow = table.AddRow();
            totalRow.Shading.Color = new MigraDoc.DocumentObjectModel.Color(255, 240, 240);
            totalRow.Cells[0].AddParagraph("TOTAL")
                .Format.Font.Bold = true;
            totalRow.Cells[1].AddParagraph(total.ToString())
                .Format.Font.Bold = true;
            totalRow.Cells[1].Format.Alignment = ParagraphAlignment.Right;
            totalRow.Cells[2].AddParagraph("100.0%")
                .Format.Font.Bold = true;
            totalRow.Cells[2].Format.Alignment = ParagraphAlignment.Right;

            section.AddParagraph().Format.SpaceAfter = "0.5cm";
        }

        /// <summary>
        /// Revenue by Area data table.
        /// </summary>
        private void AddPdfAreaRevenueSection(Section section)
        {
            if (_areaRevenue == null || !_areaRevenue.Any())
                return;

            AddSectionTitle(section, "REVENUE BY AREA");

            decimal totalRev = _areaRevenue.Sum(a => a.Revenue);

            Table table = CreateStyledTable(section,
                ("#", "1.5cm"),
                ("Area", "6cm"),
                ("Revenue (₫)", "5cm"),
                ("Share (%)", "3cm"));

            int rank = 1;

            foreach (var item in _areaRevenue)
            {
                double share = totalRev > 0
                    ? (double)(item.Revenue * 100m / totalRev)
                    : 0;

                Row row = table.AddRow();
                row.Cells[0].AddParagraph(rank.ToString());
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph(item.AreaName);
                row.Cells[2].AddParagraph($"{item.Revenue:N0}");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[3].AddParagraph($"{share:N1}%");
                row.Cells[3].Format.Alignment = ParagraphAlignment.Right;

                rank++;
            }

            section.AddParagraph().Format.SpaceAfter = "0.5cm";
        }

        /// <summary>
        /// Top Listings data table.
        /// </summary>
        private void AddPdfTopListingsSection(Section section)
        {
            if (_topListings == null || !_topListings.Any())
                return;

            AddSectionTitle(section, "TOP LISTINGS");

            Table table = CreateStyledTable(section,
                ("#", "1.5cm"),
                ("Listing", "7cm"),
                ("Revenue (₫)", "4cm"),
                ("Bookings", "3cm"));

            int rank = 1;

            foreach (var item in _topListings)
            {
                Row row = table.AddRow();
                row.Cells[0].AddParagraph(rank.ToString());
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph(item.Title);
                row.Cells[2].AddParagraph($"{item.Revenue:N0}");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[3].AddParagraph(item.BookingCount.ToString());
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

                rank++;
            }

            // Total
            Row total = table.AddRow();
            total.Shading.Color = new MigraDoc.DocumentObjectModel.Color(255, 240, 240);
            total.Cells[0].AddParagraph("");
            total.Cells[1].AddParagraph("TOTAL")
                .Format.Font.Bold = true;
            total.Cells[2].AddParagraph($"{_topListings.Sum(l => l.Revenue):N0}")
                .Format.Font.Bold = true;
            total.Cells[2].Format.Alignment = ParagraphAlignment.Right;
            total.Cells[3].AddParagraph(_topListings.Sum(l => l.BookingCount).ToString())
                .Format.Font.Bold = true;
            total.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            section.AddParagraph().Format.SpaceAfter = "0.5cm";
        }

        #endregion

        #region ══ PDF TABLE BUILDER ══

        /// <summary>
        /// Tạo section title cho PDF.
        /// </summary>
        private void AddSectionTitle(Section section, string title)
        {
            Paragraph p = section.AddParagraph();

            p.AddFormattedText(title, TextFormat.Bold);

            p.Format.Font.Size = 14;
            p.Format.Font.Color = new MigraDoc.DocumentObjectModel.Color(58, 49, 49);
            p.Format.SpaceAfter = "0.2cm";

            // Accent line
            Paragraph line = section.AddParagraph();
            line.Format.Borders.Bottom.Width = 2;
            line.Format.Borders.Bottom.Color =
                new MigraDoc.DocumentObjectModel.Color(216, 156, 157);
            line.Format.SpaceAfter = "0.3cm";
        }

        /// <summary>
        /// Tạo styled table với header row (matching app theme).
        /// </summary>
        private Table CreateStyledTable(
            Section section,
            params (string header, string width)[] columns)
        {
            Table table = section.AddTable();

            // Border style
            table.Borders.Width = 0.5;
            table.Borders.Color =
                new MigraDoc.DocumentObjectModel.Color(240, 222, 222);

            table.TopPadding = 4;
            table.BottomPadding = 4;

            // Add columns
            foreach (var col in columns)
            {
                table.AddColumn(col.width);
            }

            // Header row
            Row header = table.AddRow();
            header.Shading.Color =
                new MigraDoc.DocumentObjectModel.Color(255, 243, 243);
            header.Format.Font.Bold = true;
            header.Format.Font.Size = 11;
            header.Height = "0.8cm";
            header.VerticalAlignment =
                MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;

            for (int i = 0; i < columns.Length; i++)
            {
                header.Cells[i].AddParagraph(columns[i].header);
            }

            // Data row default style
            table.Format.Font.Size = 10;

            return table;
        }

        #endregion

        #region ══ CHART IMAGE CAPTURE ══

        /// <summary>
        /// Chụp chart WPF control thành hình ảnh và nhúng vào PDF.
        /// Nếu không chụp được thì bỏ qua (graceful fallback).
        /// </summary>
        private void AddChartImage(
            Section section,
            FrameworkElement chartControl)
        {
            string imagePath = CaptureControlImage(chartControl);

            if (imagePath == null)
                return;

            try
            {
                Paragraph imgTitle = section.AddParagraph();
                imgTitle.AddFormattedText("Chart Preview",
                    TextFormat.Italic);
                imgTitle.Format.Font.Size = 10;
                imgTitle.Format.Font.Color =
                    new MigraDoc.DocumentObjectModel.Color(139, 125, 125);
                imgTitle.Format.SpaceAfter = "0.2cm";

                var image = section.AddImage(imagePath);
                image.Width = "16cm";
                image.LockAspectRatio = true;
                image.RelativeHorizontal = RelativeHorizontal.Margin;
                image.RelativeVertical = RelativeVertical.Paragraph;
                image.WrapFormat.Style = WrapStyle.TopBottom;

                section.AddParagraph().Format.SpaceAfter = "0.4cm";
            }
            catch
            {
                // Chart capture không thành công, 
                // bỏ qua — data table vẫn có đủ thông tin
            }
        }

        /// <summary>
        /// Render WPF FrameworkElement thành PNG tạm.
        /// Dùng VisualBrush để capture SkiaSharp surface.
        /// </summary>
        //private string CaptureControlImage(FrameworkElement element)
        //{
        //    try
        //    {
        //        int width = (int)element.ActualWidth;
        //        int height = (int)element.ActualHeight;

        //        if (width <= 0 || height <= 0)
        //            return null;

        //        // Render 2x resolution cho PDF chất lượng cao
        //        int scale = 2;

        //        RenderTargetBitmap rtb =
        //            new RenderTargetBitmap(
        //                width * scale,
        //                height * scale,
        //                96 * scale,
        //                96 * scale,
        //                PixelFormats.Pbgra32);

        //        DrawingVisual dv = new DrawingVisual();

        //        using (DrawingContext dc = dv.RenderOpen())
        //        {
        //            VisualBrush vb = new VisualBrush(element)
        //            {
        //                Stretch = Stretch.None
        //            };

        //            dc.DrawRectangle(
        //                vb, null,
        //                new Rect(0, 0, width * scale, height * scale));
        //        }

        //        rtb.Render(dv);

        //        PngBitmapEncoder encoder = new PngBitmapEncoder();
        //        encoder.Frames.Add(BitmapFrame.Create(rtb));

        //        string tempPath = Path.Combine(
        //            Path.GetTempPath(),
        //            $"seoul_chart_{Guid.NewGuid():N}.png");

        //        using (FileStream fs =
        //            new FileStream(tempPath, FileMode.Create))
        //        {
        //            encoder.Save(fs);
        //        }

        //        return tempPath;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        private string CaptureControlImage(FrameworkElement element)
        {
            try
            {
                // 1. Kiểm tra kích thước thực tế của chart trên giao diện
                int width = (int)element.ActualWidth;
                int height = (int)element.ActualHeight;

                if (width <= 0 || height <= 0)
                {
                    width = 800;  // Kích thước mặc định an toàn nếu control chưa kịp render
                    height = 400;
                }

                // Tạo đường dẫn file ảnh tạm
                string tempPath = Path.Combine(
                    Path.GetTempPath(),
                    $"seoul_chart_{Guid.NewGuid():N}.png");

                // 2. Ép LiveCharts tự vẽ trực tiếp ra file ảnh dựa theo loại Chart
                if (element is CartesianChart cartesianChart)
                {
                    var skChart = new SKCartesianChart(cartesianChart)
                    {
                        Width = width * 2,  // Nhân đôi kích thước để ảnh sắc nét khi đưa vào PDF
                        Height = height * 2
                    };
                    skChart.SaveImage(tempPath);
                    return tempPath;
                }
                else if (element is PieChart pieChart)
                {
                    var skChart = new SKPieChart(pieChart)
                    {
                        Width = width * 2,
                        Height = height * 2
                    };
                    skChart.SaveImage(tempPath);
                    return tempPath;
                }

                // Fallback phương pháp cũ nếu control không phải là LiveCharts
                return CaptureControlImageFallback(element, width, height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Chart export error: {ex.Message}");
                return null;
            }
        }

        // Hàm dự phòng chuẩn hóa bằng VisualBrush cho các UI element khác (nếu có)
        private string CaptureControlImageFallback(FrameworkElement element, int width, int height)
        {
            try
            {
                double scale = 2.0;
                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)(width * scale), (int)(height * scale),
                    96.0 * scale, 96.0 * scale, PixelFormats.Pbgra32);

                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(element) { Stretch = Stretch.None };
                    dc.DrawRectangle(vb, null, new Rect(0, 0, width, height));
                }
                rtb.Render(dv);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                string tempPath = Path.Combine(Path.GetTempPath(), $"seoul_fallback_{Guid.NewGuid():N}.png");
                using (FileStream fs = new FileStream(tempPath, FileMode.Create))
                {
                    encoder.Save(fs);
                }
                return tempPath;
            }
            catch { return null; }
        }
        #endregion

        #region ══ PDF RENDER + DIALOG ══

        /// <summary>
        /// Hiện SaveFileDialog, trả về path hoặc null.
        /// </summary>
        private string ShowSaveDialog(string defaultName)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = defaultName
            };

            return dialog.ShowDialog() == true
                ? dialog.FileName
                : null;
        }

        /// <summary>
        /// Render MigraDoc Document → PDF file, rồi mở.
        /// </summary>
        private void RenderAndOpenPdf(
            Document document,
            string filePath)
        {
            PdfDocumentRenderer renderer =
                new PdfDocumentRenderer(true)
                {
                    Document = document
                };

            renderer.RenderDocument();
            renderer.PdfDocument.Save(filePath);

            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }

        private void ShowExportSuccess()
        {
            MessageBox.Show(
                "Report exported successfully!",
                "Export PDF",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ShowExportError(Exception ex)
        {
            MessageBox.Show(
                $"Export failed: {ex.Message}",
                "Export Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        #endregion
    }

    // ═════════════════════════════════════════════════════
    //  LEGEND ITEM (binding cho ItemsControl legend)
    // ═════════════════════════════════════════════════════

    public class LegendItem
    {
        public SolidColorBrush Color { get; set; }

        public string Label { get; set; }
    }
}