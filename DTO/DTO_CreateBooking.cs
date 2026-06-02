using System;
using System.Collections.Generic;

namespace DTO
{
    public class DTO_CreateBooking
    {

        // GUEST
        public long GuestUserID { get; set; }

        public string GuestFullName { get; set; }

        public string GuestEmail { get; set; }

        public string GuestPhone { get; set; }

        public bool IsNewGuest { get; set; }

        // LISTING
        public long ItemID { get; set; }

        public string ListingTitle { get; set; }

        public long HostUserID { get; set; }

        // STAY
        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }

        public string SpecialRequests { get; set; }

        // POLICY
        public long CancellationPolicyID { get; set; }

        // NIGHTS
        public List<DTO_CreateBookingNight> Nights { get; set; } = new List<DTO_CreateBookingNight>();

        // PRICING
        public decimal BaseAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal CleaningFee { get; set; }

        public decimal ServiceFee { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal FinalAmount { get; set; }

        // COUPON
        public long? CouponID { get; set; }

        public string CouponCode { get; set; }


        // PAYMENT
        public long TransactionTypeID { get; set; }

        public bool IsPaid { get; set; }

        public bool IsDeposit { get; set; }

        public decimal DepositAmount { get; set; }

        public decimal RemainingAmount
        {
            get
            {
                return FinalAmount - DepositAmount;
            }
        }

        // CREATED BY
        public long CreatedByUserID { get; set; }

        // HELPER
        public int TotalNights
        {
            get
            {
                return (CheckOutDate - CheckInDate).Days;
            }
        }

        public string StayDisplay
        {
            get
            {
                return
                    $"{CheckInDate:dd/MM/yyyy} - " +
                    $"{CheckOutDate:dd/MM/yyyy}";
            }
        }

        public string FinalAmountDisplay
        {
            get => $"{FinalAmount:N0} ₫";
        }
        public string PaymentStatus { get; set; } = "Pending";
    }
}