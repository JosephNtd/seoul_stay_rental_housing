using DTO;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class DAL_Invoice
    {
        private readonly DAL_Booking _bookingDal = new DAL_Booking();
        private readonly DAL_AddonService _addonDal = new DAL_AddonService();

        
        // INSERT INVOICE
        

        public long Insert(DTO_InvoiceCreate dto)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var invoice = new Invoice
                    {
                        GUID = Guid.NewGuid(),
                        BookingID = dto.BookingID,
                        InvoiceCode = dto.InvoiceCode,
                        IssueDate = DateTime.Now,
                        DueDate = dto.DueDate,
                        Status = "Pending",
                        TotalAmount = dto.TotalAmount,
                        PaidDate = null,
                        Notes = dto.Notes
                    };

                    db.Invoices.InsertOnSubmit(invoice);
                    db.SubmitChanges();

                    return invoice.ID;
                }
            }
            catch
            {
                return 0;
            }
        }

        
        // GET BY ID
        

        public ET_Invoices GetByID(long invoiceId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var inv = db.Invoices.FirstOrDefault(x => x.ID == invoiceId);

                if (inv == null)
                    return null;

                return new ET_Invoices
                {
                    ID = inv.ID,
                    GUID = inv.GUID,
                    BookingID = inv.BookingID,
                    InvoiceCode = inv.InvoiceCode,
                    IssueDate = inv.IssueDate,
                    DueDate = inv.DueDate,
                    Status = inv.Status,
                    TotalAmount = inv.TotalAmount,
                    PaidDate = inv.PaidDate,
                    Notes = inv.Notes
                };
            }
        }

        
        // GET BY BOOKING ID      
        public ET_Invoices GetByBookingID(long bookingId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var inv = db.Invoices.FirstOrDefault(x => x.BookingID == bookingId);

                if (inv == null)
                    return null;

                return new ET_Invoices
                {
                    ID = inv.ID,
                    GUID = inv.GUID,
                    BookingID = inv.BookingID,
                    InvoiceCode = inv.InvoiceCode,
                    IssueDate = inv.IssueDate,
                    DueDate = inv.DueDate,
                    Status = inv.Status,
                    TotalAmount = inv.TotalAmount,
                    PaidDate = inv.PaidDate,
                    Notes = inv.Notes
                };
            }
        }

        
        // UPDATE STATUS   
        public bool UpdateStatus(long invoiceId, string newStatus)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var inv = db.Invoices.FirstOrDefault(x => x.ID == invoiceId);

                    if (inv == null)
                        return false;

                    inv.Status = newStatus;

                    if (newStatus == "Paid")
                        inv.PaidDate = DateTime.Now;

                    db.SubmitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        
        // GET FULL INVOICE DTO (đầy đủ data cho PDF + Web)
        

        public DTO_Invoice GetInvoiceFull(long invoiceId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                // 1. Lấy invoice entity
                var inv = db.Invoices.FirstOrDefault(x => x.ID == invoiceId);

                if (inv == null)
                    return null;

                // 2. Lấy booking details đầy đủ từ DAL_Booking
                var booking = _bookingDal.GetBookingDetails(inv.BookingID);

                if (booking == null)
                    return null;

                // 3. Lấy coupon (nếu có)
                string couponCode = null;
                string couponName = null;

                var bookingCoupon = db.BookingCoupons
                    .FirstOrDefault(x => x.BookingID == inv.BookingID);

                if (bookingCoupon != null)
                {
                    var coupon = db.Coupons.FirstOrDefault(x => x.ID == bookingCoupon.CouponID);
                    if (coupon != null)
                    {
                        couponCode = coupon.CouponCode;
                    }
                }

                // 4. Lấy addon services
                var addons = _addonDal.GetByBookingId(inv.BookingID);

                var addonLines = addons.Select(a => new DTO_InvoiceAddon
                {
                    ServiceID = a.ServiceID,
                    ServiceName = a.ServiceName,
                    ServiceTypeName = a.ServiceTypeName,
                    UnitPrice = a.Price,
                    Quantity = a.NumberOfPeople,
                    ServiceDate = a.FromDate,
                    Notes = a.Notes
                }).ToList();

                decimal addonTotal = addonLines.Sum(x => x.LineTotal);

                // 5. Build DTO_Invoice
                var dto = new DTO_Invoice
                {
                    // INVOICE
                    InvoiceID = inv.ID,
                    InvoiceGUID = inv.GUID,
                    InvoiceCode = inv.InvoiceCode,
                    IssueDate = inv.IssueDate,
                    DueDate = inv.DueDate,
                    Status = inv.Status,
                    PaidDate = inv.PaidDate,
                    Notes = inv.Notes,

                    // HOTEL (hardcoded cho đồ án)
                    HotelName = "Seoul Stay",
                    HotelAddress = "123 Gangnam-daero, Gangnam-gu, Seoul, South Korea",
                    HotelPhone = "+82-2-1234-5678",
                    HotelEmail = "contact@seoulstay.kr",
                    HotelWebsite = "www.seoulstay.kr",

                    // BOOKING
                    BookingID = booking.BookingID,
                    BookingGUID = booking.BookingGUID,
                    BookingDate = booking.BookingDate,
                    BookingStatus = booking.BookingStatus,
                    SpecialRequests = booking.SpecialRequests,

                    // GUEST
                    GuestUserID = booking.GuestUserID,
                    GuestFullName = booking.GuestFullName,
                    GuestEmail = booking.GuestEmail,
                    GuestPhone = booking.GuestPhone,
                    GuestCountry = booking.GuestCountry,

                    // LISTING
                    ItemID = booking.ItemID,
                    ListingTitle = booking.ListingTitle,
                    ListingType = booking.ListingType,
                    ListingAddress = booking.ListingAddress,
                    AreaName = booking.AreaName,
                    HostName = booking.HostName,

                    // STAY
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    TotalNights = booking.TotalNights,
                    NumberOfGuests = booking.NumberOfGuests,

                    // NIGHTS
                    Nights = booking.Nights,

                    // ADDONS
                    Addons = addonLines,
                    AddonAmount = addonTotal,

                    // COUPON
                    CouponCode = couponCode,
                    CouponName = couponName,
                    DiscountAmount = booking.DiscountAmount,

                    // CHARGES
                    CleaningFee = booking.CleaningFee,
                    ServiceFee = booking.ServiceFee,
                    TaxAmount = booking.TaxAmount,

                    // TOTALS
                    BaseAmount = booking.TotalPrice,
                    TotalAmount = inv.TotalAmount,

                    // QR URL (BUS sẽ set sau)
                    PaymentUrl = null
                };

                return dto;
            }
        }

        
        // CHECK: BOOKING ĐÃ CÓ INVOICE CHƯA?
        

        public bool HasInvoice(long bookingId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Invoices.Any(x => x.BookingID == bookingId);
            }
        }

        
        // CREATE TRANSACTION KHI PAID (tạo bản ghi Transaction + update Booking)
        

        public bool CreatePaymentTransaction(long bookingId, decimal amount)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var booking = db.Bookings.FirstOrDefault(x => x.ID == bookingId);

                    if (booking == null)
                        return false;

                    // Tạo Transaction
                    var transaction = new Transaction
                    {
                        GUID = Guid.NewGuid(),
                        UserID = booking.GuestUserID,
                        TransactionTypeID = 1, // Payment type
                        Amount = amount,
                        TransactionDate = DateTime.Today,
                        GatewayReturnID = $"QR-{Guid.NewGuid():N}".Substring(0, 20)
                    };

                    db.Transactions.InsertOnSubmit(transaction);
                    db.SubmitChanges();

                    // Gán TransactionID vào Booking
                    booking.TransactionID = transaction.ID;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        
        // GENERATE INVOICE CODE
        

        public string GenerateNextInvoiceCode()
        {
            using (var db = new Seoul_StayDataContext())
            {
                long maxId = db.Invoices.Any()
                    ? db.Invoices.Max(x => x.ID)
                    : 0;

                return $"INV-{(maxId + 1):000000}";
            }
        }
    }
}