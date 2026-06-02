using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_ItemPrices
    {
        public List<ET_ItemPrices> GetPrices(long itemId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.ItemPrices
                         .Where(p => p.ItemID == itemId)
                         .OrderBy(p => p.Date)
                         .Select(p => new ET_ItemPrices
                         {
                             ID = p.ID,
                             GUID = p.GUID,
                             ItemID = p.ItemID,
                             Date = p.Date,
                             Price = p.Price,
                             CancellationPolicyID = p.CancellationPolicyID
                         })
                         .ToList();
            }
        }
        public bool SetPrice(long itemId, DateTime date, decimal price, long policyId)
        {
            using (var db = new Seoul_StayDataContext())
            {

                var exist = db.ItemPrices.FirstOrDefault(p => p.ItemID == itemId && p.Date == date);
                if (exist != null)
                {
                    exist.Price = price;
                    exist.CancellationPolicyID = policyId;
                }
                else
                {
                    db.ItemPrices.InsertOnSubmit(new ItemPrice
                    {
                        GUID = Guid.NewGuid(),
                        ItemID = itemId,
                        Date = date,
                        Price = price,
                        CancellationPolicyID = policyId
                    });
                    // Tự động thêm availability nếu chưa có
                    if (!db.ItemAvailabilities.Any(a => a.ItemID == itemId && a.Date == date))
                        db.ItemAvailabilities.InsertOnSubmit(new ItemAvailability
                        {
                            ItemID = itemId,
                            Date = date,
                            IsAvailable = true
                        });
                }
                db.SubmitChanges();
                return true;
            }
        }

        // Xóa giá 1 ngày
        public bool DeletePrice(long itemId, DateTime date)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var price = db.ItemPrices.FirstOrDefault(p => p.ItemID == itemId && p.Date == date);
                if (price != null)
                {
                    db.ItemPrices.DeleteOnSubmit(price);
                    // Có thể giữ availability hoặc set available = false? Tùy logic
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
        }

        // Lấy danh sách cancellation policies để điền combobox
        public List<ET_CancellationPolicies> GetPolicies()
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.CancellationPolicies
                         .Select(x => new ET_CancellationPolicies
                         {
                             ID = x.ID,
                             GUID = x.GUID,
                             Name = x.Name,
                             PlatformCommissionRate = x.PlatformCommissionRate
                         })
                         .ToList();
            }
        }

        // Availability
        public bool ToggleAvailability(long itemId, DateTime date, bool isAvailable)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var ava = db.ItemAvailabilities.FirstOrDefault(a => a.ItemID == itemId && a.Date == date);
                if (ava != null) ava.IsAvailable = isAvailable;
                else db.ItemAvailabilities.InsertOnSubmit(new ItemAvailability { ItemID = itemId, Date = date, IsAvailable = isAvailable });
                db.SubmitChanges();
                return true;
            }
        }
        public List<DateTime> GetUnavailableDates(long itemId, DateTime from, DateTime to)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.ItemAvailabilities
                         .Where(a => a.ItemID == itemId && a.Date >= from && a.Date <= to && !a.IsAvailable)
                         .Select(a => a.Date)
                         .ToList();
            }
        }
        public List<DateTime> GetBlockedDatesInSet(long itemId, List<DateTime> dates)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.ItemAvailabilities
                         .Where(a => a.ItemID == itemId && dates.Contains(a.Date) && !a.IsAvailable)
                         .Select(a => a.Date)
                         .ToList();
            }
        }

        // Kiểm tra các ngày có booking chưa hủy hay không
        public List<DateTime> GetBookedDatesInSet(long itemId, List<DateTime> dates)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var bookedRanges = db.Bookings
                    .Where(b => b.ItemID == itemId
                             && b.BookingStatus != "Cancelled" && b.BookingStatus != "Refunded"
                             && b.CheckInDate <= dates.Max() && b.CheckOutDate >= dates.Min())
                    .Select(b => new { b.CheckInDate, b.CheckOutDate })
                    .ToList();

                var bookedDates = new HashSet<DateTime>();
                foreach (var range in bookedRanges)
                {
                    for (var d = range.CheckInDate; d < range.CheckOutDate; d = d.AddDays(1))
                        if (dates.Contains(d)) bookedDates.Add(d);
                }
                return bookedDates.ToList();
            }
        }

        public List<DateTime> GetBookedDates(long itemId, DateTime from, DateTime to)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var bookedRanges = db.Bookings
                    .Where(b => b.ItemID == itemId && b.CheckOutDate >= from && b.CheckInDate <= to
                                && b.BookingStatus != "Cancelled" && b.BookingStatus != "Refunded")
                    .Select(b => new { b.CheckInDate, b.CheckOutDate })
                    .ToList();

                var bookedDates = new HashSet<DateTime>();
                foreach (var range in bookedRanges)
                {
                    for (var d = range.CheckInDate; d < range.CheckOutDate; d = d.AddDays(1))
                        if (d >= from && d <= to)
                            bookedDates.Add(d.Date);
                }
                return bookedDates.ToList();
            }
        }
        public bool SetAvailability(long itemId, DateTime date, bool isAvailable)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var ava = db.ItemAvailabilities.FirstOrDefault(a => a.ItemID == itemId && a.Date == date);
                if (ava != null)
                    ava.IsAvailable = isAvailable;
                else
                    db.ItemAvailabilities.InsertOnSubmit(new ItemAvailability
                    {
                        ItemID = itemId,
                        Date = date,
                        IsAvailable = isAvailable
                    });
                db.SubmitChanges();
                return true;
            }
        }
    }
}
