using DAL;
using DTO; 
using System;
using System.Collections.Generic;
using System.Linq;

namespace BUS
{
    public class BUS_UserBooking
    {
         
        // DAL
         

        private readonly DAL_UserBooking _dal =
            new DAL_UserBooking();

         
        // GET GUEST BOOKINGS
         

        public List<DTO_UserBookingSummary>
            GetGuestBookings(long guestUserId)
        {
            return _dal.GetGuestBookings(guestUserId);
        }

         
        // GET HOST BOOKINGS
         

        public List<DTO_UserBookingSummary>
            GetHostBookings(long hostUserId)
        {
            return _dal.GetHostBookings(hostUserId);
        }

         
        // RECENT
         

        public List<DTO_UserBookingSummary>
            GetRecentBookings(long userId)
        {
            return _dal.GetRecentBookings(userId);
        }

         
        // UPCOMING
         

        public List<DTO_UserBookingSummary>
            GetUpcomingBookings(long userId)
        {
            return _dal.GetUpcomingBookings(userId);
        }

         
        // COMPLETED
         

        public List<DTO_UserBookingSummary>
            GetCompletedBookings(long userId)
        {
            return _dal.GetCompletedBookings(userId);
        }

         
        // TOTAL SPENT
         

        public decimal GetTotalSpent(long userId)
        {
            return GetGuestBookings(userId)
                .Sum(x => x.FinalPrice);
        }

         
        // TOTAL REVENUE
         

        public decimal GetTotalRevenue(long userId)
        {
            return GetHostBookings(userId)
                .Sum(x => x.FinalPrice);
        }

         
        // UPCOMING COUNT
         

        public int GetUpcomingCount(long userId)
        {
            return GetUpcomingBookings(userId)
                .Count;
        }

         
        // COMPLETED COUNT
         

        public int GetCompletedCount(long userId)
        {
            return GetCompletedBookings(userId)
                .Count;
        }
    }
}