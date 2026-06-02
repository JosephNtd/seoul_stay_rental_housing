using DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class DAL_Report
    {
        public DTO_BookingStats GetBookingStats()
        {
            using (var db = new Seoul_StayDataContext())
            {
                DateTime today = DateTime.Today;

                var bookings =
                    db.Bookings.ToList();

                var activeBookings = bookings.Where(x => x.BookingStatus != "Cancelled" && x.BookingStatus != "Refunded").ToList();

                int totalListings = db.Items.Count(x => x.IsActive);

                int occupiedListings = activeBookings
                    .Where(x =>
                        today >= x.CheckInDate
                        &&
                        today < x.CheckOutDate)
                    .Select(x => x.ItemID)
                    .Distinct()
                    .Count();

                decimal occupancy = totalListings == 0 ? 0 : occupiedListings * 100m / totalListings;

                return new DTO_BookingStats
                {
                    TotalBookings = bookings.Count,

                    PendingBookings = bookings.Count(x => x.BookingStatus == "Pending"),

                    ConfirmedBookings = bookings.Count(x => x.BookingStatus == "Confirmed"),

                    CheckedInBookings = bookings.Count(x => x.BookingStatus == "CheckedIn"),

                    CompletedBookings = bookings.Count(x => x.BookingStatus == "Completed"),

                    CancelledBookings = bookings.Count(x => x.BookingStatus == "Cancelled"),

                    RefundedBookings = bookings.Count(x => x.BookingStatus == "Refunded"),

                    TodayCheckIns = bookings.Count(x => x.CheckInDate == today),

                    TodayCheckOuts = bookings.Count(x => x.CheckOutDate == today),

                    TodayNewBookings = bookings.Count(x => x.BookingDate.Date == today),

                    TodayCancelledBookings = bookings.Count(x => x.BookingStatus == "Cancelled" && x.BookingDate.Date == today),

                    ActiveStays = bookings.Count(x => today >= x.CheckInDate && today < x.CheckOutDate && x.BookingStatus == "CheckedIn"),

                    UpcomingBookings = bookings.Count(x => x.CheckInDate > today),

                    PastBookings = bookings.Count(x => x.CheckOutDate < today),

                    TotalAvailableListings = totalListings,

                    OccupiedListings = occupiedListings,

                    OccupancyRate = occupancy,
                    TotalRevenue = activeBookings.Sum(x => x.FinalPrice),

                    TodayRevenue = activeBookings.Where(x => x.BookingDate.Date == today).Sum(x => x.FinalPrice),

                    MonthlyRevenue = activeBookings.Where(x => x.BookingDate.Month == today.Month && x.BookingDate.Year == today.Year).Sum(x => x.FinalPrice),

                    PendingRevenue = bookings.Where(x => x.BookingStatus == "Pending").Sum(x => x.FinalPrice),

                    AverageBookingValue = activeBookings.Any() ? activeBookings.Average(x => x.FinalPrice) : 0,

                    AverageNightPrice = activeBookings.Any() ? activeBookings.Average(x => x.PricePerNight) : 0,

                    AverageStayLength = activeBookings.Any() ? activeBookings.Average(x => (decimal)(x.CheckOutDate - x.CheckInDate).Days) : 0,

                    TotalGuests = bookings.Select(x => x.GuestUserID).Distinct().Count(),

                    ReturningGuests = bookings.GroupBy(x => x.GuestUserID).Count(x => x.Count() > 1),

                    NewGuests = bookings.GroupBy(x => x.GuestUserID).Count(x => x.Count() == 1),

                    PaidBookings = bookings.Count(x => x.TransactionID != null),

                    UnpaidBookings = bookings.Count(x => x.TransactionID == null),

                    PartialPaidBookings = 0,

                    RefundedAmount = 0
                };
            }
        }

        public List<DTO_RevenueTrend> GetRevenueTrend()
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Bookings
                    .Where(x => x.BookingStatus != "Cancelled" && x.BookingStatus != "Refunded")
                    .GroupBy(x =>
                        new
                        {
                            x.BookingDate.Year,
                            x.BookingDate.Month
                        })
                    .OrderBy(x =>
                        x.Key.Year)
                    .ThenBy(x =>
                        x.Key.Month)
                    .Select(x =>
                        new DTO_RevenueTrend
                        {
                            Label = $"{x.Key.Month:00}/{x.Key.Year}",

                            Revenue = x.Sum(b => b.FinalPrice)
                        }).ToList();
            }
        }

        public List<DTO_BookingStatusReport> GetBookingStatus()
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Bookings
                    .GroupBy(x =>
                        x.BookingStatus)
                    .Select(x =>
                        new DTO_BookingStatusReport
                        {
                            Status = x.Key,
                            Count = x.Count()
                        }).OrderByDescending(x => x.Count).ToList();
            }
        }

        public List<DTO_TopListingReport> GetTopListings()
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Bookings
                    .Where(x => x.BookingStatus != "Cancelled" && x.BookingStatus != "Refunded")
                    .GroupBy(x =>
                        new
                        {
                            x.ItemID,
                            x.Item.Title
                        })
                    .Select(x =>
                        new DTO_TopListingReport
                        {
                            ItemID = x.Key.ItemID,
                            Title = x.Key.Title,
                            Revenue = x.Sum(b => b.FinalPrice),
                            BookingCount = x.Count()
                        }).OrderByDescending(x => x.Revenue).Take(10).ToList();
            }
        }

        public List<DTO_AreaRevenueReport> GetRevenueByArea()
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Bookings
                    .Where(x => x.BookingStatus != "Cancelled" && x.BookingStatus != "Refunded")
                    .GroupBy(x => x.Item.Area.Name).Select(x =>
                        new DTO_AreaRevenueReport
                        {
                            AreaName = x.Key,
                            Revenue = x.Sum(b => b.FinalPrice)
                        }).OrderByDescending(x => x.Revenue).ToList();
            }
        }
    }
}