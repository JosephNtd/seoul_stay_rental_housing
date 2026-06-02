using System;

namespace DTO
{
    public class DTO_BookingNight
    {

        // CORE IDS


        public long BookingDetailID { get; set; }

        public Guid BookingDetailGUID { get; set; }

        public long BookingID { get; set; }

        public long ItemPriceID { get; set; }


        // DATE


        public DateTime Date { get; set; }

        public string DateDisplay =>
            Date.ToString("dd MMM yyyy");

        public string DayName =>
            Date.ToString("dddd");

        public string ShortDayName =>
            Date.ToString("ddd");

        public bool IsWeekend =>
            Date.DayOfWeek == DayOfWeek.Saturday
            ||
            Date.DayOfWeek == DayOfWeek.Sunday;


        // PRICING


        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }

        // CANCELLATION / REFUND


        public bool IsRefund { get; set; }

        public DateTime? RefundDate { get; set; }

        public long? RefundCancellationPolicyID { get; set; }

        public string RefundPolicyName { get; set; }

        public decimal? RefundPenaltyPercentage { get; set; }



        // DISPLAY HELPERS


        public string BasePriceDisplay => $"${BasePrice:0.##}";

        public string FinalPriceDisplay => $"${FinalPrice:0.##}";

        // STATUS
        public string NightStatus
        {
            get
            {
                if (IsRefund)
                    return "Refunded";

                if (Date.Date < DateTime.Today)
                    return "Completed";

                if (Date.Date == DateTime.Today)
                    return "Current Stay";

                return "Upcoming";
            }
        }

        public string NightStatusColor
        {
            get
            {
                switch (NightStatus)
                {
                    case "Refunded":
                        return "#8E8E8E";

                    case "Completed":
                        return "#6C63FF";

                    case "Current Stay":
                        return "#2AA876";

                    case "Upcoming":
                        return "#3A86E9";

                    default:
                        return "#A0A0A0";
                }
            }
        }


        // FLAGS
        public bool IsToday =>
            Date.Date == DateTime.Today;

        public bool IsPast =>
            Date.Date < DateTime.Today;

        public bool IsUpcoming =>
            Date.Date > DateTime.Today;

        // SPECIAL PRICING


        public bool IsHolidayPricing { get; set; }

        public bool IsWeekendPricing { get; set; }

        public bool IsPeakSeasonPricing { get; set; }

        public string PricingTag
        {
            get
            {
                if (IsHolidayPricing)
                    return "Holiday";

                if (IsPeakSeasonPricing)
                    return "Peak Season";

                if (IsWeekendPricing)
                    return "Weekend";

                return "Standard";
            }
        }


        // SUMMARY


        public string SummaryDisplay
        {
            get
            {
                return
                    $"{Date:dd MMM} • " +
                    $"{PricingTag} • ";
            }
        }
    }
}