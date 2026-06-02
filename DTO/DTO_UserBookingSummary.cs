using System;

namespace DTO
{
    public class DTO_UserBookingSummary
    {
        // =====================================================
        // BOOKING
        // =====================================================

        public long BookingID { get; set; }

        public Guid BookingGUID { get; set; }

        public DateTime BookingDate { get; set; }

        public string BookingStatus { get; set; }

        // =====================================================
        // GUEST
        // =====================================================

        public long GuestUserID { get; set; }

        public string GuestName { get; set; }

        public string GuestEmail { get; set; }

        public string GuestPhone { get; set; }

        // =====================================================
        // HOST
        // =====================================================

        public long HostUserID { get; set; }

        public string HostName { get; set; }


        // =====================================================
        // PROPERTY
        // =====================================================

        public long ItemID { get; set; }

        public string ItemTitle { get; set; }

        public string ItemThumbnail { get; set; }

        public string AreaName { get; set; }
        public string Address { get; set; }

        public string PropertyType { get; set; }
        public string ItemTypeName { get; set; }

        // =====================================================
        // STAY
        // =====================================================

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int Nights { get; set; }

        public int GuestCount { get; set; }

        // =====================================================
        // FINANCIAL
        // =====================================================

        public decimal BasePrice { get; set; }

        public decimal CleaningFee { get; set; }

        public decimal ServiceFee { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalPrice { get; set; }

        // =====================================================
        // PAYMENT
        // =====================================================

        public long? TransactionID { get; set; }

        public string TransactionType { get; set; }

        public DateTime? TransactionDate { get; set; }

        // =====================================================
        // CANCELLATION
        // =====================================================

        public string CancellationPolicy { get; set; }

        public bool IsRefundable { get; set; }

        public decimal? RefundAmount { get; set; }

        // =====================================================
        // REVIEW
        // =====================================================

        public bool HasReview { get; set; }

        public decimal? Rating { get; set; }

        // =====================================================
        // STATUS DISPLAY
        // =====================================================

        public string StatusDisplay
        {
            get
            {
                switch (BookingStatus)
                {
                    case "Pending":
                        return "Pending Approval";

                    case "Confirmed":
                        return "Confirmed";

                    case "CheckedIn":
                        return "Checked In";

                    case "Completed":
                        return "Completed";

                    case "Cancelled":
                        return "Cancelled";

                    default:
                        return "Unknown";
                }
            }
        }

        // =====================================================
        // STATUS COLOR
        // =====================================================

        public string StatusColor
        {
            get
            {
                switch (BookingStatus)
                {
                    case "Pending":
                        return "#F5B041";

                    case "Confirmed":
                        return "#5DADE2";

                    case "CheckedIn":
                        return "#48C9B0";

                    case "Completed":
                        return "#58D68D";

                    case "Cancelled":
                        return "#EC7063";

                    default:
                        return "#B2BABB";
                }
            }
        }

        // =====================================================
        // DATE RANGE
        // =====================================================

        public string StayDateDisplay
        {
            get
            {
                return
                    $"{CheckInDate:dd MMM yyyy} - " +
                    $"{CheckOutDate:dd MMM yyyy}";
            }
        }

        // =====================================================
        // NIGHT DISPLAY
        // =====================================================

        public string NightDisplay
        {
            get
            {
                return $"{Nights} night(s)";
            }
        }

        // =====================================================
        // PRICE DISPLAY
        // =====================================================

        public string FinalPriceDisplay
        {
            get
            {
                return $"${FinalPrice:N0}";
            }
        }

        // =====================================================
        // REVIEW DISPLAY
        // =====================================================

        public string ReviewDisplay
        {
            get
            {
                if (!HasReview)
                    return "No Review";

                return $"{Rating:0.0} ★";
            }
        }

        // =====================================================
        // REFUND DISPLAY
        // =====================================================

        public string RefundDisplay
        {
            get
            {
                if (RefundAmount == null)
                    return "-";

                return $"${RefundAmount:N0}";
            }
        }

        // =====================================================
        // BOOKING TYPE
        // =====================================================

        public bool IsUpcoming
        {
            get
            {
                return CheckInDate > DateTime.Now;
            }
        }

        public bool IsCurrentStay
        {
            get
            {
                DateTime now = DateTime.Now;

                return now >= CheckInDate
                    && now <= CheckOutDate;
            }
        }

        public bool IsPastStay
        {
            get
            {
                return CheckOutDate < DateTime.Now;
            }
        }

        // =====================================================
        // UI FLAGS
        // =====================================================

        public bool CanCancel
        {
            get
            {
                return BookingStatus == "Pending"
                    || BookingStatus == "Confirmed";
            }
        }

        public bool CanReview
        {
            get
            {
                return BookingStatus == "Completed"
                    && !HasReview;
            }
        }

        public bool CanRefund
        {
            get
            {
                return BookingStatus == "Cancelled"
                    && RefundAmount != null
                    && RefundAmount > 0;
            }
        }
    }
}