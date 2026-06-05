using DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class DAL_AddonService
    {


        /// <summary>
        /// CREATE ADDON HEADER + DETAILS
        /// Được gọi từ DAL_Booking.CreateManualBooking
        /// trong cùng TransactionScope, dùng chung db context
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bookingId"></param>
        /// <param name="userId"></param>
        /// <param name="couponId"></param>
        /// <param name="addonList"></param>
        /// <returns></returns>


        public long CreateAddonWithDetails(Seoul_StayDataContext db, long bookingId, long userId, long? couponId, List<DTO_CreateBookingAddon> addonList)
        {
            // 1. Tạo AddonServices header

            var header = new AddonService
            {
                GUID = Guid.NewGuid(),
                UserID = userId,
                BookingID = bookingId,
                CouponID = couponId
            };

            db.AddonServices.InsertOnSubmit(header);

            db.SubmitChanges();

            // 2. Tạo từng AddonServiceDetail

            foreach (var addon in addonList)
            {
                var detail = new AddonServiceDetail
                {
                    GUID = Guid.NewGuid(),
                    AddonServiceID = header.ID,
                    ServiceID = addon.ServiceID,
                    Price = addon.Price,
                    FromDate = addon.FromDate,
                    NumberOfPeople = addon.NumberOfPeople,
                    Notes = addon.Notes ?? "",
                    isRefund = false
                };

                db.AddonServiceDetails.InsertOnSubmit(detail);
            }

            db.SubmitChanges();

            return header.ID;
        }


        // ĐẾM SỐ LƯỢNG ĐÃ ĐẶT THEO NGÀY (cho DailyCap)
        // Đếm tất cả AddonServiceDetail có cùng ServiceID
        // và cùng FromDate (chưa bị refund)


        public long CountDailyUsage(long serviceId, DateTime fromDate)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.AddonServiceDetails
                         .Where(x => x.ServiceID == serviceId
                                  && x.FromDate.Date == fromDate.Date
                                  && x.isRefund == false)
                         .Sum(x => (long?)x.NumberOfPeople) ?? 0;
            }
        }


        // ĐẾM SỐ LƯỢNG ĐÃ ĐẶT THEO BOOKING (cho BookingCap)
        // Đếm số lần service xuất hiện trong 1 booking cụ thể


        public int CountBookingUsage(long serviceId, long bookingId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var addonIds = db.AddonServices
                                 .Where(x => x.BookingID == bookingId)
                                 .Select(x => x.ID)
                                 .ToList();

                if (!addonIds.Any())
                    return 0;

                return db.AddonServiceDetails
                         .Count(x => addonIds.Contains(x.AddonServiceID)
                                  && x.ServiceID == serviceId
                                  && x.isRefund == false);
            }
        }


        // LẤY DANH SÁCH ADDON THEO BOOKING (cho hiển thị)


        public List<DTO_CreateBookingAddon> GetByBookingId(long bookingId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var addonHeader = db.AddonServices
                                    .FirstOrDefault(x => x.BookingID == bookingId);

                if (addonHeader == null)
                    return new List<DTO_CreateBookingAddon>();

                var query = from d in db.AddonServiceDetails
                            join s in db.Services
                                on d.ServiceID equals s.ID
                            join st in db.ServiceTypes
                                on s.ServiceTypeID equals st.ID
                            where d.AddonServiceID == addonHeader.ID
                               && d.isRefund == false
                            select new DTO_CreateBookingAddon
                            {
                                ServiceID = d.ServiceID,
                                ServiceName = s.Name,
                                ServiceTypeName = st.Name,
                                Price = d.Price,
                                NumberOfPeople = d.NumberOfPeople,
                                FromDate = d.FromDate,
                                Notes = d.Notes
                            };

                return query.ToList();
            }
        }
    }
}