using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System.IO;
using System.Xml.Linq;

namespace Helper
{
    public class Helper_InvoicePDF
    {
        /// <summary>
        /// Build PDF document từ DTO_Invoice và export ra file.
        /// </summary>
        /// <param name="invoice">DTO chứa toàn bộ dữ liệu invoice</param>
        /// <param name="qrPngBytes">QR Code dưới dạng PNG byte[] (từ Helper_QRCode.GeneratePngBytes)</param>
        /// <param name="outputPath">Đường dẫn file PDF output</param>
        public static void ExportPdf(DTO.DTO_Invoice invoice, byte[] qrPngBytes, string outputPath)
        {
            Document document = BuildDocument(invoice, qrPngBytes);

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(outputPath);
        }

        /// <summary>
        /// Build PDF document và trả về byte[] (cho trường hợp không dùng SaveFileDialog).
        /// </summary>
        public static byte[] ExportPdfBytes(DTO.DTO_Invoice invoice, byte[] qrPngBytes)
        {
            Document document = BuildDocument(invoice, qrPngBytes);

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();

            using (MemoryStream ms = new MemoryStream())
            {
                renderer.PdfDocument.Save(ms, false);
                return ms.ToArray();
            }
        }

        // =============================================================
        // BUILD DOCUMENT
        // =============================================================

        private static Document BuildDocument(DTO.DTO_Invoice invoice, byte[] qrPngBytes)
        {
            Document document = new Document();
            document.Info.Title = $"Invoice {invoice.InvoiceCode}";
            document.Info.Author = invoice.HotelName;

            // Page setup
            Section section = document.AddSection();
            section.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1.5);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(2);
            section.PageSetup.RightMargin = Unit.FromCentimeter(2);

            // 1. HEADER — Hotel Info
            AddHotelHeader(section, invoice);

            // 2. INVOICE INFO
            AddInvoiceInfo(section, invoice);

            // 3. GUEST INFO
            AddGuestInfo(section, invoice);

            // 4. LISTING + STAY INFO
            AddListingInfo(section, invoice);

            // 5. NIGHT PRICING TABLE
            AddNightPricingTable(section, invoice);

            // 6. ADDON SERVICES TABLE
            AddAddonTable(section, invoice);

            // 7. SUMMARY
            AddSummary(section, invoice);

            // 8. QR CODE
            AddQrSection(section, invoice, qrPngBytes);

            // 9. FOOTER
            AddFooter(section, invoice);

            return document;
        }

        // =============================================================
        // 1. HOTEL HEADER
        // =============================================================

        private static void AddHotelHeader(Section section, DTO.DTO_Invoice invoice)
        {
            Paragraph title = section.AddParagraph(invoice.HotelName);
            title.Format.Font.Size = 20;
            title.Format.Font.Bold = true;
            title.Format.Font.Color = Colors.DarkSlateBlue;
            title.Format.SpaceAfter = Unit.FromPoint(4);

            Paragraph address = section.AddParagraph(invoice.HotelAddress);
            address.Format.Font.Size = 9;
            address.Format.Font.Color = Colors.Gray;

            Paragraph contact = section.AddParagraph(
                $"Phone: {invoice.HotelPhone}  |  Email: {invoice.HotelEmail}");
            contact.Format.Font.Size = 9;
            contact.Format.Font.Color = Colors.Gray;
            contact.Format.SpaceAfter = Unit.FromPoint(12);

            // Divider
            Paragraph divider = section.AddParagraph("─────────────────────────────────────────────────────────────────");
            divider.Format.Font.Size = 6;
            divider.Format.Font.Color = Colors.LightGray;
            divider.Format.SpaceAfter = Unit.FromPoint(10);
        }

        // =============================================================
        // 2. INVOICE INFO
        // =============================================================

        private static void AddInvoiceInfo(Section section, DTO.DTO_Invoice invoice)
        {
            Paragraph header = section.AddParagraph("INVOICE");
            header.Format.Font.Size = 14;
            header.Format.Font.Bold = true;
            header.Format.SpaceAfter = Unit.FromPoint(6);

            Table table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(8));

            AddInfoRow(table, "Invoice Code:", invoice.InvoiceCode);
            AddInfoRow(table, "Issue Date:", invoice.IssueDateDisplay);
            AddInfoRow(table, "Due Date:", invoice.DueDateDisplay);
            AddInfoRow(table, "Status:", invoice.StatusDisplay);
            AddInfoRow(table, "Booking Code:", invoice.BookingCode);
            AddInfoRow(table, "Booking Date:", invoice.BookingDateDisplay);

            section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(10);
        }

        // =============================================================
        // 3. GUEST INFO
        // =============================================================

        private static void AddGuestInfo(Section section, DTO.DTO_Invoice invoice)
        {
            Paragraph header = section.AddParagraph("GUEST INFORMATION");
            header.Format.Font.Size = 11;
            header.Format.Font.Bold = true;
            header.Format.SpaceAfter = Unit.FromPoint(4);

            Table table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(8));

            AddInfoRow(table, "Full Name:", invoice.GuestFullName);
            AddInfoRow(table, "Email:", invoice.GuestEmail ?? "—");
            AddInfoRow(table, "Phone:", invoice.GuestPhone ?? "—");
            AddInfoRow(table, "Country:", invoice.GuestCountry ?? "—");

            section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(10);
        }

        // =============================================================
        // 4. LISTING + STAY INFO
        // =============================================================

        private static void AddListingInfo(Section section, DTO.DTO_Invoice invoice)
        {
            Paragraph header = section.AddParagraph("LISTING & STAY");
            header.Format.Font.Size = 11;
            header.Format.Font.Bold = true;
            header.Format.SpaceAfter = Unit.FromPoint(4);

            Table table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(8));

            AddInfoRow(table, "Listing:", invoice.ListingTitle);
            AddInfoRow(table, "Type:", invoice.ListingType);
            AddInfoRow(table, "Area:", invoice.AreaName ?? "—");
            AddInfoRow(table, "Address:", invoice.ListingAddress ?? "—");
            AddInfoRow(table, "Host:", invoice.HostName);
            AddInfoRow(table, "Check-in:", invoice.CheckInDisplay);
            AddInfoRow(table, "Check-out:", invoice.CheckOutDisplay);
            AddInfoRow(table, "Nights:", invoice.NightDisplay);
            AddInfoRow(table, "Guests:", invoice.GuestDisplay);

            section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(10);
        }

        // =============================================================
        // 5. NIGHT PRICING TABLE
        // =============================================================

        private static void AddNightPricingTable(Section section, DTO.DTO_Invoice invoice)
        {
            if (invoice.Nights == null || invoice.Nights.Count == 0)
                return;

            Paragraph header = section.AddParagraph("NIGHT PRICING BREAKDOWN");
            header.Format.Font.Size = 11;
            header.Format.Font.Bold = true;
            header.Format.SpaceAfter = Unit.FromPoint(6);

            Table table = section.AddTable();
            table.Borders.Width = 0.5;
            table.Borders.Color = Colors.LightGray;

            // Columns
            table.AddColumn(Unit.FromCentimeter(1));   // #
            table.AddColumn(Unit.FromCentimeter(4));   // Date
            table.AddColumn(Unit.FromCentimeter(3));   // Day
            table.AddColumn(Unit.FromCentimeter(4));   // Price

            // Header row
            Row headerRow = table.AddRow();
            headerRow.Shading.Color = new Color(245, 245, 245);
            headerRow.Format.Font.Bold = true;
            headerRow.Format.Font.Size = 9;
            headerRow.Cells[0].AddParagraph("#");
            headerRow.Cells[1].AddParagraph("Date");
            headerRow.Cells[2].AddParagraph("Day");
            headerRow.Cells[3].AddParagraph("Price");
            headerRow.Cells[3].Format.Alignment = ParagraphAlignment.Right;

            // Data rows
            for (int i = 0; i < invoice.Nights.Count; i++)
            {
                var night = invoice.Nights[i];

                Row row = table.AddRow();
                row.Format.Font.Size = 9;

                row.Cells[0].AddParagraph((i + 1).ToString());
                row.Cells[1].AddParagraph(night.DateDisplay);
                row.Cells[2].AddParagraph(night.DayName);

                Paragraph priceCell = row.Cells[3].AddParagraph($"${night.BasePrice:0.##}");
                priceCell.Format.Alignment = ParagraphAlignment.Right;

                // Highlight weekend
                if (night.IsWeekend)
                {
                    row.Shading.Color = new Color(255, 250, 240);
                }
            }

            section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(10);
        }

        // =============================================================
        // 6. ADDON SERVICES TABLE
        // =============================================================

        private static void AddAddonTable(Section section, DTO.DTO_Invoice invoice)
        {
            if (invoice.Addons == null || invoice.Addons.Count == 0)
                return;

            Paragraph header = section.AddParagraph("ADD-ON SERVICES");
            header.Format.Font.Size = 11;
            header.Format.Font.Bold = true;
            header.Format.SpaceAfter = Unit.FromPoint(6);

            Table table = section.AddTable();
            table.Borders.Width = 0.5;
            table.Borders.Color = Colors.LightGray;

            // Columns
            table.AddColumn(Unit.FromCentimeter(4.5)); // Service
            table.AddColumn(Unit.FromCentimeter(2.5)); // Date
            table.AddColumn(Unit.FromCentimeter(1.5)); // Qty
            table.AddColumn(Unit.FromCentimeter(2));   // Unit Price
            table.AddColumn(Unit.FromCentimeter(2));   // Total

            // Header row
            Row headerRow = table.AddRow();
            headerRow.Shading.Color = new Color(245, 245, 245);
            headerRow.Format.Font.Bold = true;
            headerRow.Format.Font.Size = 9;
            headerRow.Cells[0].AddParagraph("Service");
            headerRow.Cells[1].AddParagraph("Date");
            headerRow.Cells[2].AddParagraph("Qty");
            headerRow.Cells[3].AddParagraph("Unit Price");
            headerRow.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            headerRow.Cells[4].AddParagraph("Total");
            headerRow.Cells[4].Format.Alignment = ParagraphAlignment.Right;

            // Data rows
            foreach (var addon in invoice.Addons)
            {
                Row row = table.AddRow();
                row.Format.Font.Size = 9;

                row.Cells[0].AddParagraph(addon.ServiceName);
                row.Cells[1].AddParagraph(addon.ServiceDateDisplay);
                row.Cells[2].AddParagraph(addon.QuantityDisplay);

                Paragraph unitCell = row.Cells[3].AddParagraph(addon.UnitPriceDisplay);
                unitCell.Format.Alignment = ParagraphAlignment.Right;

                Paragraph totalCell = row.Cells[4].AddParagraph(addon.LineTotalDisplay);
                totalCell.Format.Alignment = ParagraphAlignment.Right;
            }

            section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(10);
        }

        // =============================================================
        // 7. SUMMARY
        // =============================================================

        private static void AddSummary(Section section, DTO.DTO_Invoice invoice)
        {
            Paragraph header = section.AddParagraph("PAYMENT SUMMARY");
            header.Format.Font.Size = 11;
            header.Format.Font.Bold = true;
            header.Format.SpaceAfter = Unit.FromPoint(6);

            Table table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(8));
            table.AddColumn(Unit.FromCentimeter(4));

            AddSummaryRow(table, "Subtotal (Nights):", invoice.BaseAmountDisplay, false);

            if (invoice.AddonAmount > 0)
                AddSummaryRow(table, "Add-on Services:", invoice.AddonAmountDisplay, false);

            if (invoice.CleaningFee > 0)
                AddSummaryRow(table, "Cleaning Fee:", invoice.CleaningFeeDisplay, false);

            if (invoice.ServiceFee > 0)
                AddSummaryRow(table, "Service Fee:", invoice.ServiceFeeDisplay, false);

            if (invoice.TaxAmount > 0)
                AddSummaryRow(table, "Tax:", invoice.TaxDisplay, false);

            if (invoice.HasDiscount)
                AddSummaryRow(table, $"Discount ({invoice.CouponDisplay}):", invoice.DiscountDisplay, false);

            // Divider row
            Row dividerRow = table.AddRow();
            dividerRow.Cells[0].AddParagraph("───────────────────────────────────");
            dividerRow.Cells[0].Format.Font.Size = 6;
            dividerRow.Cells[0].Format.Font.Color = Colors.LightGray;
            dividerRow.Cells[0].MergeRight = 1;

            // TOTAL
            AddSummaryRow(table, "TOTAL AMOUNT:", invoice.TotalAmountDisplay, true);

            section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(14);
        }

        // =============================================================
        // 8. QR CODE SECTION
        // =============================================================

        private static void AddQrSection(Section section, DTO.DTO_Invoice invoice, byte[] qrPngBytes)
        {
            if (qrPngBytes == null || qrPngBytes.Length == 0)
                return;

            // Divider
            Paragraph divider = section.AddParagraph("─────────────────────────────────────────────────────────────────");
            divider.Format.Font.Size = 6;
            divider.Format.Font.Color = Colors.LightGray;
            divider.Format.SpaceAfter = Unit.FromPoint(10);

            Paragraph qrHeader = section.AddParagraph("PAYMENT QR CODE");
            qrHeader.Format.Font.Size = 11;
            qrHeader.Format.Font.Bold = true;
            qrHeader.Format.Alignment = ParagraphAlignment.Center;
            qrHeader.Format.SpaceAfter = Unit.FromPoint(8);

            // Save QR to temp file (MigraDoc 1.50 requires file path for images)
            string tempQrPath = Path.Combine(Path.GetTempPath(), $"invoice_qr_{invoice.InvoiceID}.png");
            File.WriteAllBytes(tempQrPath, qrPngBytes);

            Paragraph imgParagraph = section.AddParagraph();
            imgParagraph.Format.Alignment = ParagraphAlignment.Center;

            var image = imgParagraph.AddImage(tempQrPath);
            image.Width = Unit.FromCentimeter(5);
            image.Height = Unit.FromCentimeter(5);

            Paragraph qrText = section.AddParagraph("Scan to simulate payment");
            qrText.Format.Font.Size = 10;
            qrText.Format.Font.Italic = true;
            qrText.Format.Font.Color = Colors.Gray;
            qrText.Format.Alignment = ParagraphAlignment.Center;
            qrText.Format.SpaceBefore = Unit.FromPoint(6);

            if (!string.IsNullOrWhiteSpace(invoice.PaymentUrl))
            {
                Paragraph urlText = section.AddParagraph(invoice.PaymentUrl);
                urlText.Format.Font.Size = 8;
                urlText.Format.Font.Color = Colors.DarkBlue;
                urlText.Format.Alignment = ParagraphAlignment.Center;
                urlText.Format.SpaceBefore = Unit.FromPoint(4);
            }
        }

        // =============================================================
        // 9. FOOTER
        // =============================================================

        private static void AddFooter(Section section, DTO.DTO_Invoice invoice)
        {
            section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(20);

            Paragraph footer = section.AddParagraph(
                $"This is a computer-generated invoice. No signature required.");
            footer.Format.Font.Size = 8;
            footer.Format.Font.Color = Colors.Gray;
            footer.Format.Font.Italic = true;
            footer.Format.Alignment = ParagraphAlignment.Center;

            Paragraph generated = section.AddParagraph(
                $"Generated on {invoice.IssueDate:dd MMM yyyy HH:mm} by {invoice.HotelName}");
            generated.Format.Font.Size = 8;
            generated.Format.Font.Color = Colors.Gray;
            generated.Format.Alignment = ParagraphAlignment.Center;
        }

        // =============================================================
        // HELPERS
        // =============================================================

        private static void AddInfoRow(Table table, string label, string value)
        {
            Row row = table.AddRow();
            row.Format.Font.Size = 9;

            Paragraph labelCell = row.Cells[0].AddParagraph(label);
            labelCell.Format.Font.Bold = true;
            labelCell.Format.Font.Color = Colors.DarkGray;

            row.Cells[1].AddParagraph(value ?? "—");
        }

        private static void AddSummaryRow(Table table, string label, string value, bool isBold)
        {
            Row row = table.AddRow();
            row.Format.Font.Size = isBold ? 11 : 9;

            Paragraph labelCell = row.Cells[0].AddParagraph(label);
            labelCell.Format.Font.Bold = isBold;
            labelCell.Format.Alignment = ParagraphAlignment.Right;

            Paragraph valueCell = row.Cells[1].AddParagraph(value);
            valueCell.Format.Alignment = ParagraphAlignment.Right;
            valueCell.Format.Font.Bold = isBold;

            if (isBold)
            {
                valueCell.Format.Font.Color = Colors.DarkSlateBlue;
            }
        }
    }
}