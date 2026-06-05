using DAL;
using DTO;
using System;

namespace BUS
{
    public class BUS_Invoice
    {
        private readonly DAL_Invoice _invoiceDal = new DAL_Invoice();
        private readonly DAL_Booking _bookingDal = new DAL_Booking();

        
        // GENERATE INVOICE (tạo invoice mới cho 1 booking)
        

        /// <summary>
        /// Tạo invoice cho booking đã tạo thành công.
        /// Trả về invoiceId nếu thành công, 0 nếu thất bại.
        /// </summary>
        public long GenerateInvoice(long bookingId, out string error)
        {
            error = string.Empty;

            // 1. Validate: booking phải tồn tại
            var booking = _bookingDal.GetBookingEntity(bookingId);

            if (booking == null)
            {
                error = "Booking not found.";
                return 0;
            }

            // 2. Validate: booking chưa có invoice
            if (_invoiceDal.HasInvoice(bookingId))
            {
                error = "Invoice already exists for this booking.";
                return 0;
            }

            // 3. Validate: booking chưa bị hủy
            if (booking.BookingStatus == "Cancelled" || booking.BookingStatus == "Refunded")
            {
                error = "Cannot create invoice for cancelled or refunded booking.";
                return 0;
            }

            // 4. Generate invoice code
            string invoiceCode = _invoiceDal.GenerateNextInvoiceCode();

            // 5. Build DTO_InvoiceCreate
            var createDto = new DTO_InvoiceCreate
            {
                BookingID = bookingId,
                InvoiceCode = invoiceCode,
                TotalAmount = booking.FinalPrice,
                DueDate = DateTime.Now.AddDays(3),
                Notes = null
            };

            // 6. Insert
            long invoiceId = _invoiceDal.Insert(createDto);

            if (invoiceId <= 0)
            {
                error = "Unable to create invoice.";
                return 0;
            }

            return invoiceId;
        }

        
        // GET FULL INVOICE DTO (đầy đủ cho PDF + Web)
        

        /// <summary>
        /// Lấy invoice đầy đủ dữ liệu để render PDF hoặc hiển thị Web.
        /// Tự fill PaymentUrl dựa trên baseUrl truyền vào.
        /// </summary>
        public DTO_Invoice GetInvoiceFull(long invoiceId, string baseUrl)
        {
            var dto = _invoiceDal.GetInvoiceFull(invoiceId);

            if (dto == null)
                return null;

            // Fill payment URL cho QR
            dto.PaymentUrl = $"{baseUrl.TrimEnd('/')}/payment/{invoiceId}";

            return dto;
        }

        
        // GET INVOICE BY BOOKING
        

        public DTO_Invoice GetInvoiceByBookingId(long bookingId, string baseUrl)
        {
            var entity = _invoiceDal.GetByBookingID(bookingId);

            if (entity == null)
                return null;

            return GetInvoiceFull(entity.ID, baseUrl);
        }

        
        // CONFIRM PAYMENT (gọi từ Web khi user nhấn Confirm)
        

        /// <summary>
        /// Xác nhận thanh toán: cập nhật Invoice = Paid, tạo Transaction, link vào Booking.
        /// </summary>
        public bool ConfirmPayment(long invoiceId, out string error)
        {
            error = string.Empty;

            // 1. Lấy invoice hiện tại
            var invoice = _invoiceDal.GetByID(invoiceId);

            if (invoice == null)
            {
                error = "Invoice not found.";
                return false;
            }

            // 2. Chỉ cho phép thanh toán nếu invoice đang Pending
            if (invoice.Status != "Pending")
            {
                error = $"Cannot confirm payment. Invoice is already '{invoice.Status}'.";
                return false;
            }

            // 3. Cập nhật invoice status → Paid
            bool statusUpdated = _invoiceDal.UpdateStatus(invoiceId, "Paid");

            if (!statusUpdated)
            {
                error = "Failed to update invoice status.";
                return false;
            }

            // 4. Tạo Transaction + gán vào Booking.TransactionID
            bool transactionCreated = _invoiceDal.CreatePaymentTransaction(
                invoice.BookingID, invoice.TotalAmount);

            if (!transactionCreated)
            {
                error = "Failed to create payment transaction.";
                return false;
            }

            // 5. Cập nhật booking status → Confirmed (nếu đang Pending)
            var booking = _bookingDal.GetBookingEntity(invoice.BookingID);

            if (booking != null && booking.BookingStatus == "Pending")
            {
                _bookingDal.UpdateBookingStatus(invoice.BookingID, "Confirmed");
                _bookingDal.InsertBookingTimeline(
                    invoice.BookingID,
                    "Pending",
                    "Confirmed",
                    null,
                    "Auto-confirmed via QR payment");
            }

            return true;
        }

        
        // CANCEL PAYMENT (gọi từ Web khi user nhấn Cancel)
        

        /// <summary>
        /// Hủy thanh toán: cập nhật Invoice = Cancelled.
        /// Không hủy booking — chỉ hủy invoice.
        /// </summary>
        public bool CancelPayment(long invoiceId, out string error)
        {
            error = string.Empty;

            var invoice = _invoiceDal.GetByID(invoiceId);

            if (invoice == null)
            {
                error = "Invoice not found.";
                return false;
            }

            if (invoice.Status != "Pending")
            {
                error = $"Cannot cancel. Invoice is already '{invoice.Status}'.";
                return false;
            }

            bool updated = _invoiceDal.UpdateStatus(invoiceId, "Cancelled");

            if (!updated)
            {
                error = "Failed to cancel invoice.";
                return false;
            }

            return true;
        }

        
        /// <summary>
        /// CHECK: CÓ THỂ GENERATE INVOICE KHÔNG?
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        

        public bool CanGenerateInvoice(long bookingId)
        {
            var booking = _bookingDal.GetBookingEntity(bookingId);

            if (booking == null)
                return false;

            if (booking.BookingStatus == "Cancelled" || booking.BookingStatus == "Refunded")
                return false;

            // Đã có invoice rồi thì không tạo thêm
            if (_invoiceDal.HasInvoice(bookingId))
                return false;

            // Đã có transaction (thanh toán trực tiếp) thì không cần invoice
            if (booking.TransactionID != null)
                return false;

            return true;
        }
    }
}