using System;
using System.Collections.Generic;

namespace DTO
{
    public class DTO_Invoice
    {
        
        // INVOICE CORE
        

        public long InvoiceID { get; set; }

        public Guid InvoiceGUID { get; set; }

        public string InvoiceCode { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? DueDate { get; set; }

        /// <summary>Pending | Paid | Cancelled | Expired</summary>
        public string Status { get; set; }

        public DateTime? PaidDate { get; set; }

        public string Notes { get; set; }

        
        // INVOICE STATUS DISPLAY
        

        public bool IsPending => Status == "Pending";
        public bool IsPaid => Status == "Paid";
        public bool IsCancelled => Status == "Cancelled";
        public bool IsExpired => Status == "Expired";

        public string StatusDisplay
        {
            get
            {
                switch (Status)
                {
                    case "Pending": return "Awaiting Payment";
                    case "Paid": return "Paid";
                    case "Cancelled": return "Cancelled";
                    case "Expired": return "Expired";
                    default: return "Unknown";
                }
            }
        }

        public string IssueDateDisplay => IssueDate.ToString("dd MMM yyyy HH:mm");
        public string PaidDateDisplay => PaidDate?.ToString("dd MMM yyyy HH:mm") ?? "—";
        public string DueDateDisplay => DueDate?.ToString("dd MMM yyyy HH:mm") ?? "—";

        
        // HOTEL / SYSTEM INFO  (hiển thị header PDF)
        

        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelPhone { get; set; }
        public string HotelEmail { get; set; }
        public string HotelWebsite { get; set; }

        
        // BOOKING CORE
        

        public long BookingID { get; set; }
        public Guid BookingGUID { get; set; }
        public string BookingCode => $"BK-{BookingID:000000}";

        public DateTime BookingDate { get; set; }
        public string BookingDateDisplay => BookingDate.ToString("dd MMM yyyy HH:mm");

        public string BookingStatus { get; set; }
        public string BookingStatusDisplay
        {
            get
            {
                switch (BookingStatus)
                {
                    case "Pending": return "Pending";
                    case "Confirmed": return "Confirmed";
                    case "CheckedIn": return "Checked-In";
                    case "Completed": return "Completed";
                    case "Cancelled": return "Cancelled";
                    case "Refunded": return "Refunded";
                    default: return "Unknown";
                }
            }
        }

        public string SpecialRequests { get; set; }

        
        // GUEST INFO
        

        public long GuestUserID { get; set; }
        public string GuestFullName { get; set; }
        public string GuestEmail { get; set; }
        public string GuestPhone { get; set; }
        public string GuestCountry { get; set; }

        
        // LISTING INFO
        

        public long ItemID { get; set; }
        public string ListingTitle { get; set; }
        public string ListingType { get; set; }
        public string ListingAddress { get; set; }
        public string AreaName { get; set; }
        public string HostName { get; set; }

        
        // STAY INFO
        

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }
        public int NumberOfGuests { get; set; }

        public string CheckInDisplay => CheckInDate.ToString("dd MMM yyyy");
        public string CheckOutDisplay => CheckOutDate.ToString("dd MMM yyyy");
        public string NightDisplay => $"{TotalNights} night(s)";
        public string GuestDisplay => $"{NumberOfGuests} guest(s)";
        public string DateRangeDisplay => $"{CheckInDisplay} → {CheckOutDisplay}";

        
        // NIGHT PRICING BREAKDOWN
        

        /// <summary>Chi tiết từng đêm — render thành bảng trong PDF</summary>
        public List<DTO_BookingNight> Nights { get; set; } = new List<DTO_BookingNight>();

        
        // ADD-ON SERVICES
        

        /// <summary>Danh sách dịch vụ bổ sung — render thành bảng riêng trong PDF</summary>
        public List<DTO_InvoiceAddon> Addons { get; set; } = new List<DTO_InvoiceAddon>();

        
        // DISCOUNT / COUPON
        

        public string CouponCode { get; set; }
        public string CouponName { get; set; }
        public decimal DiscountAmount { get; set; }

        public bool HasDiscount => DiscountAmount > 0;
        public string DiscountDisplay => HasDiscount ? $"-${DiscountAmount:0.##}" : "—";
        public string CouponDisplay => string.IsNullOrWhiteSpace(CouponCode) ? "—" : CouponCode;

        
        // CHARGES
        

        public decimal CleaningFee { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TaxAmount { get; set; }

        public string CleaningFeeDisplay => $"${CleaningFee:0.##}";
        public string ServiceFeeDisplay => $"${ServiceFee:0.##}";
        public string TaxDisplay => $"${TaxAmount:0.##}";

        
        // SUMMARY / TOTALS
        

        public decimal BaseAmount { get; set; }   // tổng tiền đêm trước mọi điều chỉnh
        public decimal AddonAmount { get; set; }   // tổng add-on services
        public decimal TotalAmount { get; set; }   // FinalPrice = base + addon + fees + tax - discount

        public string BaseAmountDisplay => $"${BaseAmount:0.##}";
        public string AddonAmountDisplay => AddonAmount > 0 ? $"${AddonAmount:0.##}" : "—";
        public string TotalAmountDisplay => $"${TotalAmount:0.##}";

        /// <summary>Subtotal = BaseAmount + AddonAmount (trước phí và giảm giá)</summary>
        public decimal Subtotal => BaseAmount + AddonAmount;
        public string SubtotalDisplay => $"${Subtotal:0.##}";

        
        // QR / PAYMENT URL
        

        /// <summary>URL nhúng vào QR Code, ví dụ: http://192.168.1.x:5000/payment/42</summary>
        public string PaymentUrl { get; set; }
    }

    
    // NESTED: Add-on line item cho invoice
    

    public class DTO_InvoiceAddon
    {
        public long ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string ServiceTypeName { get; set; }
        public decimal UnitPrice { get; set; }
        public long Quantity { get; set; }
        public DateTime ServiceDate { get; set; }
        public string Notes { get; set; }

        public decimal LineTotal => UnitPrice * Quantity;
        public string UnitPriceDisplay => $"${UnitPrice:0.##}";
        public string LineTotalDisplay => $"${LineTotal:0.##}";
        public string ServiceDateDisplay => ServiceDate.ToString("dd MMM yyyy");
        public string QuantityDisplay => $"x{Quantity}";
    }
}