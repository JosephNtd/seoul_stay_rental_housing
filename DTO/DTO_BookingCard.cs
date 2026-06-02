using System;

namespace DTO
{
    public class DTO_BookingCard
    {
         
        // CORE IDS
        public long BookingID { get; set; }

        public Guid BookingGUID { get; set; }

        public long GuestUserID { get; set; }

        public long ItemID { get; set; }

        public long? TransactionID { get; set; }

         
        // GUEST INFO
        public string GuestFullName { get; set; }

        public string GuestEmail { get; set; }

        public string GuestPhone { get; set; }

        public string GuestCountry { get; set; }

        public string GuestAvatar { get; set; }

        public bool IsGuestVerified { get; set; }

         
        // LISTING INFO
        public string ListingTitle { get; set; }

        public string ListingType { get; set; }

        public string ListingAddress { get; set; }

        public string ListingThumbnail { get; set; }

        public string HostName { get; set; }

         
        // BOOKING INFO
        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }

        public int TotalNights { get; set; }

        public DateTime BookingDate { get; set; }

        public string SpecialRequests { get; set; }

         
        // PRICING
        public decimal PricePerNight { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalPrice { get; set; }

        public decimal? RefundAmount { get; set; }

         
        // BOOKING STATUS
        public string BookingStatus { get; set; }

        public bool IsPending => BookingStatus == "Pending";

        public bool IsConfirmed => BookingStatus == "Confirmed";

        public bool IsCheckedIn => BookingStatus == "CheckedIn";

        public bool IsCompleted => BookingStatus == "Completed";

        public bool IsCancelled => BookingStatus == "Cancelled";

        public bool IsRefunded => BookingStatus == "Refunded";

         
        // PAYMENT
        public bool HasTransaction => TransactionID != null;

        public bool IsPaid => TransactionID != null && !IsCancelled;

        public string PaymentStatus
        {
            get
            {
                if (IsRefunded)
                    return "Refunded";

                if (IsCancelled)
                    return "Cancelled";

                if (HasTransaction)
                    return "Paid";

                return "Unpaid";
            }
        }

         
        // DISPLAY HELPERS
        public string BookingCode => $"BK-{BookingID:000000}";

        public string DateRangeDisplay => $"{CheckInDate:dd MMM yyyy} → {CheckOutDate:dd MMM yyyy}";

        public string NightDisplay => $"{TotalNights} night(s)";

        public string GuestDisplay => $"{NumberOfGuests} guest(s)";

        public string FinalPriceDisplay => $"${FinalPrice:0.##}";

        public string BookingDateDisplay => BookingDate.ToString(
                "dd MMM yyyy HH:mm");

        public string StatusDisplay
        {
            get
            {
                switch (BookingStatus)
                {
                    case "Pending":
                        return "Pending";

                    case "Confirmed":
                        return "Confirmed";

                    case "CheckedIn":
                        return "Checked-In";

                    case "Completed":
                        return "Completed";

                    case "Cancelled":
                        return "Cancelled";

                    case "Refunded":
                        return "Refunded";

                    default:
                        return "Unknown";
                }
            }
        }

         
        // UI COLORS
        public string StatusColor
        {
            get
            {
                switch (BookingStatus)
                {
                    case "Pending":
                        return "#E9A63A";

                    case "Confirmed":
                        return "#3A86E9";

                    case "CheckedIn":
                        return "#2AA876";

                    case "Completed":
                        return "#6C63FF";

                    case "Cancelled":
                        return "#E05252";

                    case "Refunded":
                        return "#8E8E8E";

                    default:
                        return "#A0A0A0";
                }
            }
        }

        public string PaymentColor
        {
            get
            {
                switch (PaymentStatus)
                {
                    case "Paid":
                        return "#2AA876";

                    case "Refunded":
                        return "#8E8E8E";

                    case "Cancelled":
                        return "#E05252";

                    default:
                        return "#E9A63A";
                }
            }
        }

         
        // QUICK OPERATIONS
        public bool CanConfirm => IsPending;

        public bool CanCheckIn => IsConfirmed && DateTime.Today >= CheckInDate.Date;

        public bool CanCheckOut => IsCheckedIn;

        public bool CanCancel => !IsCancelled && !IsCompleted;

        public bool CanRefund => IsCancelled && HasTransaction;

         
        // OCCUPANCY / CALENDAR
        public bool IsActiveStay
        {
            get
            {
                DateTime today = DateTime.Today;

                return today >= CheckInDate.Date && today <= CheckOutDate.Date && !IsCancelled;
            }
        }

        public bool IsUpcoming => DateTime.Today < CheckInDate.Date;

        public bool IsPast => DateTime.Today > CheckOutDate.Date;

         
        // PRIORITY FLAGS
        public bool RequiresAttention => IsPending || (IsConfirmed && DateTime.Today == CheckInDate.Date);

        public bool IsTodayCheckIn => DateTime.Today == CheckInDate.Date;

        public bool IsTodayCheckOut => DateTime.Today == CheckOutDate.Date;
    }
}