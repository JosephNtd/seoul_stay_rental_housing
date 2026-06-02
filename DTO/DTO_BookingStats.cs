using System;

namespace DTO
{
    public class DTO_BookingStats
    {
         
        // TOTAL BOOKINGS
         

        public int TotalBookings { get; set; }

        public int PendingBookings { get; set; }

        public int ConfirmedBookings { get; set; }

        public int CheckedInBookings { get; set; }

        public int CompletedBookings { get; set; }

        public int CancelledBookings { get; set; }

        public int RefundedBookings { get; set; }

         
        // TODAY OPERATIONS
         

        public int TodayCheckIns { get; set; }

        public int TodayCheckOuts { get; set; }

        public int TodayNewBookings { get; set; }

        public int TodayCancelledBookings { get; set; }

         
        // ACTIVE STAYS
         

        public int ActiveStays { get; set; }

        public int UpcomingBookings { get; set; }

        public int PastBookings { get; set; }

         
        // OCCUPANCY
         

        public int TotalAvailableListings { get; set; }

        public int OccupiedListings { get; set; }

        public decimal OccupancyRate { get; set; }

         
        // REVENUE
         

        public decimal TotalRevenue { get; set; }

        public decimal TodayRevenue { get; set; }

        public decimal MonthlyRevenue { get; set; }

        public decimal PendingRevenue { get; set; }

        public decimal RefundedAmount { get; set; }

         
        // AVERAGES
         

        public decimal AverageBookingValue { get; set; }

        public decimal AverageNightPrice { get; set; }

        public decimal AverageStayLength { get; set; }

         
        // GUESTS
         

        public int TotalGuests { get; set; }

        public int ReturningGuests { get; set; }

        public int NewGuests { get; set; }

         
        // PAYMENT
         

        public int PaidBookings { get; set; }

        public int UnpaidBookings { get; set; }

        public int PartialPaidBookings { get; set; }

         
        // DISPLAY HELPERS
         

        public string OccupancyDisplay =>
            $"{OccupancyRate:0.#}%";

        public string TotalRevenueDisplay =>
            $"${TotalRevenue:0,0.##}";

        public string TodayRevenueDisplay =>
            $"${TodayRevenue:0,0.##}";

        public string MonthlyRevenueDisplay =>
            $"${MonthlyRevenue:0,0.##}";

        public string PendingRevenueDisplay =>
            $"${PendingRevenue:0,0.##}";

        public string RefundedAmountDisplay =>
            $"${RefundedAmount:0,0.##}";

        public string AverageBookingDisplay =>
            $"${AverageBookingValue:0,0.##}";

        public string AverageNightDisplay =>
            $"${AverageNightPrice:0,0.##}";

        public string AverageStayDisplay =>
            $"{AverageStayLength:0.#} nights";

         
        // KPI FLAGS
         

        public bool HasPendingBookings =>
            PendingBookings > 0;

        public bool HasTodayCheckIns =>
            TodayCheckIns > 0;

        public bool HasTodayCheckOuts =>
            TodayCheckOuts > 0;

        public bool HasActiveStays =>
            ActiveStays > 0;

        public bool HasRefunds =>
            RefundedAmount > 0;

         
        // PERFORMANCE LEVELS
         

        public string OccupancyLevel
        {
            get
            {
                if (OccupancyRate >= 90)
                    return "Excellent";

                if (OccupancyRate >= 75)
                    return "High";

                if (OccupancyRate >= 50)
                    return "Medium";

                return "Low";
            }
        }

        public string RevenueLevel
        {
            get
            {
                if (MonthlyRevenue >= 50000)
                    return "Excellent";

                if (MonthlyRevenue >= 20000)
                    return "Strong";

                if (MonthlyRevenue >= 10000)
                    return "Average";

                return "Low";
            }
        }

         
        // UI COLORS
         

        public string OccupancyColor
        {
            get
            {
                if (OccupancyRate >= 90)
                    return "#2AA876";

                if (OccupancyRate >= 75)
                    return "#3A86E9";

                if (OccupancyRate >= 50)
                    return "#E9A63A";

                return "#E05252";
            }
        }

        public string RevenueColor
        {
            get
            {
                if (MonthlyRevenue >= 50000)
                    return "#2AA876";

                if (MonthlyRevenue >= 20000)
                    return "#3A86E9";

                if (MonthlyRevenue >= 10000)
                    return "#E9A63A";

                return "#E05252";
            }
        }

         
        // TREND FLAGS
         

        public bool IsHighOccupancy =>
            OccupancyRate >= 80;

        public bool IsHealthyRevenue =>
            MonthlyRevenue >= 10000;

        public bool RequiresAttention =>
            PendingBookings > 10
            ||
            OccupancyRate < 40;

         
        // SUMMARY TEXT
         

        public string SummaryDisplay
        {
            get
            {
                return
                    $"{ActiveStays} active stays • " +
                    $"{OccupancyDisplay} occupancy • " +
                    $"{TodayRevenueDisplay} today";
            }
        }
    }
}