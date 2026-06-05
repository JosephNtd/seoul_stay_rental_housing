using System;

namespace DTO
{
    /// <summary>
    /// Input object để tạo Invoice mới từ một Booking đã tồn tại.
    /// BUS_Invoice nhận object này, validate, rồi gọi DAL_Invoice.Insert().
    /// </summary>
    public class DTO_InvoiceCreate
    {
        
        // REQUIRED: liên kết booking
        

        public long BookingID { get; set; }

        
        // AMOUNTS  (copy từ DTO_CreateBooking / DTO_BookingDetails)
        

        public decimal BaseAmount { get; set; }
        public decimal AddonAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CleaningFee { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TaxAmount { get; set; }

        /// <summary>Số tiền cuối cùng ghi lên invoice = base + addon + fees + tax - discount</summary>
        public decimal TotalAmount { get; set; }

        
        // OPTIONAL
        

        public DateTime? DueDate { get; set; }

        public string Notes { get; set; }

        
        // COMPUTED (BUS tự generate — không cần caller truyền)
        

        /// <summary>
        /// BUS_Invoice.GenerateInvoice() sẽ tự set field này.
        /// Format: INV-{BookingID:000000}-{yyyyMMdd}
        /// </summary>
        public string InvoiceCode { get; set; }
    }
}