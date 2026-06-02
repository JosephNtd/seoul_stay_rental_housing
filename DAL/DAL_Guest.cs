using DTO;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Guest
    {
        public ET_Guest GetGuestProfile(long userId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var query = from g in db.Guests
                            join u in db.Users on g.UserID equals u.ID
                            where g.UserID == userId
                            select new ET_Guest
                            {
                                // Các thuộc tính từ Users
                                ID = u.ID,
                                GUID = u.GUID,
                                Username = u.Username,
                                Password = u.Password,
                                FullName = u.FullName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                Gender = u.Gender,
                                BirthDate = u.BirthDate,
                                Country = u.Country,
                                ProfilePicture = u.ProfilePicture,

                                // Các thuộc tính từ Hosts
                                LoyaltyPoints = g.LoyaltyPoints,
                                PreferredLanguage = g.PreferredLanguage,
                                NationalID = g.NationalID,
                                NationalIDVerified = g.NationalIDVerified,
                            };
                return query.FirstOrDefault();
            }
        }
        public bool UpdateGuestProfile(ET_Guest guest)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var user = db.Users.FirstOrDefault(u => u.ID == guest.ID);
                var g = db.Guests.FirstOrDefault(x => x.UserID == guest.ID);
                if (user == null || g == null) return false;

                user.FullName = guest.FullName;
                user.Email = guest.Email;
                user.PhoneNumber = guest.PhoneNumber;
                user.Gender = guest.Gender;
                user.BirthDate = guest.BirthDate;
                user.Country = guest.Country;
                // Không cho sửa Username, Password

                g.PreferredLanguage = guest.PreferredLanguage;
                string oldNationalID = g.NationalID;
                g.NationalID = guest.NationalID;
                if (oldNationalID != guest.NationalID)
                    g.NationalIDVerified = false;

                db.SubmitChanges();
                return true;
            }
        }
        public List<DTO_RecentStay> GetRecentStays(long userId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data =
                    (
                        from b in db.Bookings
                        join i in db.Items
                            on b.ItemID equals i.ID

                        where b.GuestUserID == userId
                              && b.BookingStatus != "Cancelled"

                        orderby b.BookingDate descending

                        select new DTO_RecentStay
                        {
                            BookingID = b.ID,

                            Title = i.Title,

                            ThumbnailPath =
                                db.ItemPictures
                                  .Where(p => p.ItemID == i.ID)
                                  .OrderBy(p => p.DisplayOrder)
                                  .Select(p => p.PictureFileName)
                                  .FirstOrDefault(),

                            CheckInDate = b.CheckInDate,

                            CheckOutDate = b.CheckOutDate
                        }

                    ).Take(6).ToList();

                // ADD FAKE CARD

                data.Add(new DTO_RecentStay
                {
                    IsAddCard = true
                });

                return data;
            }
        }
        public List<DTO_GuestLookup> GetLookupData()
        {
            using (var db = new Seoul_StayDataContext())
            {
                return
                    (from g in db.Guests
                     join u in db.Users
                         on g.UserID equals u.ID
                     orderby u.FullName
                     select new DTO_GuestLookup
                     {
                         UserID = u.ID,
                         FullName = u.FullName,
                         Email = u.Email,
                         PhoneNumber = u.PhoneNumber,
                         Country = u.Country
                     })
                    .ToList();
            }
        }
    }
}
