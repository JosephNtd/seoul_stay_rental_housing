using System;
using System.Collections.Generic;

namespace DTO
{
    public class DTO_BookingDetails
    {

        // CORE BOOKING
        public long BookingID { get; set; }
        public Guid BookingGUID { get; set; }
        public string BookingCode => $"BK-{BookingID:000000}";
        public DateTime BookingDate { get; set; }
        public string BookingDateDisplay => BookingDate.ToString("dd MMM yyyy HH:mm");


        // GUEST
        public long GuestUserID { get; set; }
        public string GuestFullName { get; set; }
        public string GuestEmail { get; set; }
        public string GuestPhone { get; set; }
        public string GuestCountry { get; set; }
        public DateTime? GuestBirthDate { get; set; }
        public string GuestAvatar { get; set; }
        public bool IsGuestVerified { get; set; }
        public int LoyaltyPoints { get; set; }
        public string PreferredLanguage { get; set; }
        public string NationalID { get; set; }

        // LISTING
        public long ItemID { get; set; }
        public string ListingTitle { get; set; }
        public string ListingType { get; set; }
        public string ListingThumbnail { get; set; }
        public string ListingDescription { get; set; }
        public string ListingAddress { get; set; }
        public string AreaName { get; set; }
        public int Capacity { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfBedrooms { get; set; }
        public int NumberOfBathrooms { get; set; }
        public string HostRules { get; set; }


        // HOST
        public long HostUserID { get; set; }
        public string HostName { get; set; }
        public string HostEmail { get; set; }
        public string HostPhone { get; set; }
        public string HostAvatar { get; set; }
        public bool IsHostVerified { get; set; }
        public decimal? HostRating { get; set; }

        // BOOKING INFO
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int TotalNights { get; set; }
        public string SpecialRequests { get; set; }

        // BOOKING STATUS
        public string BookingStatus { get; set; }
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

        public bool IsPending => BookingStatus == "Pending";
        public bool IsConfirmed => BookingStatus == "Confirmed";
        public bool IsCheckedIn => BookingStatus == "CheckedIn";
        public bool IsCompleted => BookingStatus == "Completed";
        public bool IsCancelled => BookingStatus == "Cancelled";
        public bool IsRefunded => BookingStatus == "Refunded";

        // PRICING
        public decimal PricePerNight { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal? RefundAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal CleaningFee { get; set; }
        public decimal TaxAmount { get; set; }


        // PAYMENT


        public long? TransactionID { get; set; }
        public string TransactionCode => TransactionID == null ? "N/A" : $"TRX-{TransactionID:000000}";
        public DateTime? TransactionDate { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string GatewayReturnID { get; set; }
        public bool HasPayment =>
            TransactionID != null;

        public string PaymentStatus
        {
            get
            {
                if (IsRefunded)
                    return "Refunded";

                if (IsCancelled)
                    return "Cancelled";

                if (HasPayment)
                    return "Paid";

                return "Unpaid";
            }
        }


        // CANCELLATION


        public long CancellationPolicyID { get; set; }
        public string CancellationPolicyName { get; set; }
        public decimal PlatformCommissionRate { get; set; }
        public int? RefundDaysLeft { get; set; }
        public decimal? RefundPenaltyPercentage { get; set; }


        // DISPLAY HELPERS
        public string DateRangeDisplay => $"{CheckInDate:dd MMM yyyy} → {CheckOutDate:dd MMM yyyy}";
        public string NightDisplay => $"{TotalNights} night(s)";
        public string GuestDisplay => $"{NumberOfGuests} guest(s)";
        public string FinalPriceDisplay => $"${FinalPrice:0.##}";
        public string PricePerNightDisplay => $"${PricePerNight:0.##} / night";
        public string DiscountDisplay => $"-${DiscountAmount:0.##}";
        public string RefundDisplay => RefundAmount == null ? "$0" : $"${RefundAmount:0.##}";
        public string TaxDisplay => $"${TaxAmount:0.##}";
        public string CleaningFeeDisplay => $"${CleaningFee:0.##}";
        public string ServiceFeeDisplay => $"${ServiceFee:0.##}";


        // QUICK OPERATIONS
        public bool CanConfirm => IsPending;
        public bool CanCheckIn => IsConfirmed && DateTime.Today >= CheckInDate.Date;
        public bool CanCheckOut => IsCheckedIn;
        public bool CanCancel => !IsCancelled && !IsCompleted;
        public bool CanRefund => IsCancelled && HasPayment;

        // STAY FLAGS
        public bool IsUpcoming => DateTime.Today < CheckInDate.Date;
        public bool IsActiveStay
        {
            get
            {
                DateTime today = DateTime.Today;

                return today >= CheckInDate.Date
                    &&
                       today <= CheckOutDate.Date
                    &&
                       !IsCancelled;
            }
        }

        public bool IsPast => DateTime.Today > CheckOutDate.Date;
        public bool IsTodayCheckIn => DateTime.Today == CheckInDate.Date;
        public bool IsTodayCheckOut => DateTime.Today == CheckOutDate.Date;


        // RELATED COLLECTIONS

        public List<DTO_BookingTimeline> Timeline { get; set; } = new List<DTO_BookingTimeline>();
        public List<DTO_BookingNight> Nights { get; set; } = new List<DTO_BookingNight>();
        public List<string> Amenities { get; set; } = new List<string>();
        public List<string> Attractions { get; set; } = new List<string>();
        public List<string> Pictures { get; set; } = new List<string>();
    }
}