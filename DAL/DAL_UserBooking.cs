using DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class DAL_UserBooking
    {
        // GUEST BOOKINGS

        public List<DTO_UserBookingSummary>
            GetGuestBookings(long guestUserId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var query =
                    from b in db.Bookings

                    join i in db.Items
                        on b.ItemID equals i.ID

                    join guest in db.Users
                        on b.GuestUserID equals guest.ID

                    join host in db.Users
                        on i.HostUserID equals host.ID
                    join itemType in db.ItemTypes
                        on i.ItemTypeID equals itemType.ID

                    select new DTO_UserBookingSummary
                    {
                         
                        // BOOKING
                         

                        BookingID = b.ID,
                        BookingGUID = b.GUID,

                        BookingDate = b.BookingDate,

                        BookingStatus =
                            b.BookingStatus,
                        // GUEST
                        GuestUserID = guest.ID,
                        GuestName = guest.FullName,
                        GuestEmail = guest.Email,

                        GuestPhone = guest.PhoneNumber,
                        // HOST
                        HostUserID = host.ID,
                        HostName = host.FullName,
                         
                        // ITEM
                        ItemID = i.ID,
                        ItemTitle = i.Title,
                        Address = i.ExactAddress,

                        ItemTypeName = itemType.Name,


                        // STAY
                        CheckInDate = b.CheckInDate,

                        CheckOutDate = b.CheckOutDate,

                        Nights =
                            (b.CheckOutDate - b.CheckInDate)
                            .Days,

                        GuestCount =
                            b.NumberOfGuests,

                         
                        // MONEY
                         

                        FinalPrice =
                            b.TotalPrice,

                         
                        // TRANSACTION
                         

                        TransactionID =
                            b.TransactionID

                    };

                return query
                    .Where(x =>
                        x.GuestUserID == guestUserId)
                    .OrderByDescending(x =>
                        x.BookingDate)
                    .ToList();
            }
        }

        // HOST BOOKINGS

        public List<DTO_UserBookingSummary>
            GetHostBookings(long hostUserId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var query =
                    from b in db.Bookings

                    join i in db.Items
                        on b.ItemID equals i.ID

                    join guest in db.Users
                        on b.GuestUserID equals guest.ID

                    join host in db.Users
                        on i.HostUserID equals host.ID

                    select new DTO_UserBookingSummary
                    {
                         
                        // BOOKING
                         

                        BookingID = b.ID,
                        BookingGUID = b.GUID,

                        BookingDate = b.BookingDate,

                        BookingStatus =
                            b.BookingStatus,

                         
                        // GUEST
                         

                        GuestUserID = guest.ID,
                        GuestName = guest.FullName,

                         
                        // HOST
                         

                        HostUserID = host.ID,
                        HostName = host.FullName,

                         
                        // ITEM
                         

                        ItemID = i.ID,
                        ItemTitle = i.Title,

                         
                        // STAY
                         

                        CheckInDate = b.CheckInDate,

                        CheckOutDate = b.CheckOutDate,

                        Nights =
                            (b.CheckOutDate - b.CheckInDate)
                            .Days,

                        GuestCount =
                            b.NumberOfGuests,

                         
                        // MONEY
                         

                        FinalPrice =
                            b.TotalPrice,

                         
                        // TRANSACTION
                         

                        TransactionID =
                            b.TransactionID
                    };

                return query
                    .Where(x =>
                        x.HostUserID == hostUserId)
                    .OrderByDescending(x =>
                        x.BookingDate)
                    .ToList();
            }
        }

        // RECENT

        public List<DTO_UserBookingSummary>
            GetRecentBookings(long userId)
        {
            var guest =
                GetGuestBookings(userId);

            var host =
                GetHostBookings(userId);

            return guest
                .Concat(host)
                .OrderByDescending(x =>
                    x.BookingDate)
                .Take(10)
                .ToList();
        }

        // UPCOMING

        public List<DTO_UserBookingSummary>
            GetUpcomingBookings(long userId)
        {
            return GetRecentBookings(userId)
                .Where(x =>
                    x.CheckInDate > DateTime.Now)
                .ToList();
        }

        // COMPLETED

        public List<DTO_UserBookingSummary>
            GetCompletedBookings(long userId)
        {
            return GetRecentBookings(userId)
                .Where(x =>
                    x.BookingStatus == "Completed")
                .ToList();
        }
    }
}
