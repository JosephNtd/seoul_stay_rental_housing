using System;
using System.Collections.Generic;
using System.Linq;
using ET;

namespace DAL
{
    public class DAL_ItemAvailability
    {
        /// <summary>
        /// Lấy danh sách trạng thái của Item trong khoảng thời gian để vẽ lên Lịch
        /// </summary>
        public List<ET_ItemAvailability> GetAvailabilities(long itemId, DateTime startDate, DateTime endDate)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var query = from a in db.ItemAvailabilities
                            where a.ItemID == itemId
                               && a.Date >= startDate.Date
                               && a.Date <= endDate.Date
                            select new ET_ItemAvailability
                            {
                                ID = a.ID,
                                ItemID = a.ItemID,
                                Date = a.Date,
                                IsAvailable = a.IsAvailable
                            };

                return query.ToList();
            }
        }
        // Hàm lấy toàn bộ trạng thái ngày của một ItemId
        public List<ET_ItemAvailability> GetAvailabilities(long itemId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.ItemAvailabilities
                         .Where(a => a.ItemID == itemId)
                         .Select(a => new ET_ItemAvailability
                         {
                             ID = a.ID,
                             ItemID = a.ItemID,
                             Date = a.Date,
                             IsAvailable = a.IsAvailable
                         }).ToList();
            }
        }
        public bool SetAvailabilities(long itemId, List<DateTime> dates, bool isAvailable)
        {
            if (dates == null || !dates.Any()) return false;

            using (var db = new Seoul_StayDataContext())
            {
                try
                {
                    // Chuẩn hóa danh sách ngày về dạng không chứa giờ (.Date)
                    var dateList = dates.Select(d => d.Date).ToList();

                    // Lấy nhanh các bản ghi đã từng có trong DB để chuẩn bị UPDATE
                    var existingRecords = db.ItemAvailabilities
                                            .Where(a => a.ItemID == itemId && dateList.Contains(a.Date))
                                            .ToList();

                    foreach (var date in dateList)
                    {
                        var record = existingRecords.FirstOrDefault(a => a.Date == date);
                        if (record != null)
                        {
                            // Nếu ngày này đã có bản ghi -> Cập nhật trạng thái mới
                            record.IsAvailable = isAvailable;
                        }
                        else
                        {
                            // Nếu ngày này chưa có -> Thêm mới bản ghi vào bảng
                            var newRecord = new ItemAvailability
                            {
                                ItemID = itemId,
                                Date = date,
                                IsAvailable = isAvailable
                            };

                            db.ItemAvailabilities.InsertOnSubmit(newRecord);

                        }
                    }

                    db.SubmitChanges();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}