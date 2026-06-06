using DTO;
using ET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DAL
{
    public class DAL_Items
    {

        public string LastError { get; private set; }

        public List<DTO_ItemsView> GetItems(long? userId = null)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var query = from i in db.Items
                            join a in db.Areas on i.AreaID equals a.ID
                            join t in db.ItemTypes on i.ItemTypeID equals t.ID
                            where i.IsActive
                            select new { Item = i, Area = a, Type = t };

                if (userId.HasValue)
                {
                    query = query.Where(x => x.Item.HostUserID == userId.Value);
                }

                return query
                    .Select(x => new DTO_ItemsView
                    {
                        ID = x.Item.ID,
                        Title = x.Item.Title,
                        Capacity = x.Item.Capacity,
                        Area = x.Area.Name,
                        Type = x.Type.Name
                    })
                    .ToList();
            }
        }

        // DAL_Items.cs
        public List<DTO_ItemCard> GetItemCards(long? hostUserId = null)
        {
            using (var db = new Seoul_StayDataContext())
            {
                // Khai báo ngày hôm nay ra một biến riêng trước câu lệnh LINQ.
                // Ép LINQ to SQL truyền vào DB một tham số ngày cố định (00:00:00), không bị dính giờ phút giây của GETDATE().
                DateTime today = DateTime.Today;

                // Lọc Items theo Host ngay từ đầu (nếu có) để câu lệnh SQL gọn nhẹ hơn
                var itemsQuery = db.Items.AsQueryable();
                if (hostUserId.HasValue)
                {
                    itemsQuery = itemsQuery.Where(i => i.HostUserID == hostUserId.Value);
                }

                var query = from i in itemsQuery
                            join t in db.ItemTypes on i.ItemTypeID equals t.ID
                            where i.IsActive

                            let minPrice = db.ItemPrices
                                .Where(p => p.ItemID == i.ID && p.Date >= today)
                                .Min(p => (decimal?)p.Price)

                            let firstPic = db.ItemPictures
                                .Where(pic => pic.ItemID == i.ID)
                                .OrderBy(pic => pic.DisplayOrder)
                                .Select(pic => pic.PictureFileName)
                                .FirstOrDefault()

                            // logic kiểm tra phòng có đang bị chiếm dụng/đang ở HÔM NAY hay không
                            let isOccupiedToday = db.Bookings
                                .Any(b => b.ItemID == i.ID &&
                                          today >= b.CheckInDate &&   // Hôm nay phải bằng hoặc sau ngày Check-in
                                          today < b.CheckOutDate &&   // Hôm nay phải trước ngày Check-out (vì ngày checkout phòng trống từ trưa)
                                          (b.BookingStatus == "Pending"
                                          || b.BookingStatus == "Confirmed"
                                          || b.BookingStatus == "CheckedIn"))

                            select new DTO_ItemCard
                            {
                                ID = i.ID,
                                Title = i.Title,
                                ApproximateAddress = i.ApproximateAddress ?? "Unknown",
                                Type = t.Name,
                                Capacity = i.Capacity,
                                NumberOfBeds = i.NumberOfBeds,
                                NumberOfBathrooms = i.NumberOfBathrooms,
                                MinPrice = minPrice,
                                ThumbnailPath = firstPic,
                                Status = isOccupiedToday ? "Occupied" :
                                         (minPrice == null ? "Draft" : "Active")
                            };

                return query.ToList();
            }
        }

        public bool Add(Item i)
        {
            using (var db = new Seoul_StayDataContext())
            {
                try
                {
                    if (i.GUID == Guid.Empty)
                    {
                        i.GUID = Guid.NewGuid();
                    }

                    if (i.CreatedDate == DateTime.MinValue)
                    {
                        i.CreatedDate = DateTime.Now;
                    }

                    i.IsActive = true;
                    db.Items.InsertOnSubmit(i);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    return false;
                }
            }
        }

        public bool Add(ET_Items item, IEnumerable<long> amenityIds, IEnumerable<DTO_Attraction_Distance> attractions)
        {
            LastError = null;

            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    OpenConnection(db);

                    using (var transaction = db.Connection.BeginTransaction())
                    {
                        db.Transaction = transaction;

                        var newItem = CreateItemEntity(db, item);
                        db.Items.InsertOnSubmit(newItem);
                        db.SubmitChanges();

                        ReplaceItemAmenities(db, newItem.ID, amenityIds);
                        ReplaceItemAttractions(db, newItem.ID, attractions);
                        db.SubmitChanges();

                        transaction.Commit();
                        item.ID = newItem.ID;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public bool Update(ET_Items item, IEnumerable<long> amenityIds, IEnumerable<DTO_Attraction_Distance> attractions)
        {
            LastError = null;

            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    OpenConnection(db);

                    using (var transaction = db.Connection.BeginTransaction())
                    {
                        db.Transaction = transaction;

                        var current = db.Items.FirstOrDefault(x => x.ID == item.ID);
                        if (current == null)
                        {
                            LastError = "Item not found.";
                            return false;
                        }

                        // 1. Update main info
                        UpdateItemEntity(db, current, item);

                        // 2. Update amenities
                        ReplaceItemAmenities(db, current.ID, amenityIds);

                        // 3. Update attractions
                        ReplaceItemAttractions(db, current.ID, attractions);

                        db.SubmitChanges();

                        transaction.Commit();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public ET_Items GetEditItem(long id)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var i = db.Items.FirstOrDefault(x => x.ID == id);
                if (i == null) return null;

                return new ET_Items
                {
                    ID = i.ID,
                    GUID = i.GUID,
                    HostUserID = i.HostUserID,
                    ItemTypeID = i.ItemTypeID,
                    AreaID = i.AreaID,
                    Title = i.Title,
                    Capacity = i.Capacity,
                    NumberOfBeds = i.NumberOfBeds,
                    NumberOfBedrooms = i.NumberOfBedrooms,
                    NumberOfBathrooms = i.NumberOfBathrooms,
                    ExactAddress = i.ExactAddress,
                    ApproximateAddress = i.ApproximateAddress,
                    Description = i.Description,
                    HostRules = i.HostRules,
                    MinimumNights = i.MinimumNights,
                    MaximumNights = i.MaximumNights
                };
            }
        }

        public long GetAreaIdByApproximateAddress(string approximateAddress)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return ResolveAreaId(db, approximateAddress, 0);
            }
        }

        public List<ET_Amenities> GetEditItemAmenities(long id)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data = from ia in db.ItemAmenities
                           join a in db.Amenities on ia.AmenityID equals a.ID
                           where ia.ItemID == id
                           select new ET_Amenities
                           {
                               ID = a.ID,
                               AmenitiesName = a.Name
                           };
                return data.ToList();
            }
        }

        private static void OpenConnection(Seoul_StayDataContext db)
        {
            if (db.Connection.State != ConnectionState.Open)
            {
                db.Connection.Open();
            }
        }

        private static Item CreateItemEntity(Seoul_StayDataContext db, ET_Items source)
        {
            var item = new Item();
            UpdateItemEntity(db, item, source);
            item.GUID = Guid.NewGuid();
            item.HostUserID = source.HostUserID;
            item.CreatedDate = DateTime.Now;
            item.IsActive = true;
            return item;
        }

        private static void UpdateItemEntity(Seoul_StayDataContext db, Item target, ET_Items source)
        {
            var areaId = ResolveAreaId(db, source.ApproximateAddress, source.AreaID);
            if (areaId <= 0)
            {
                throw new InvalidOperationException("Approximate Address must match an existing area.");
            }

            target.ItemTypeID = source.ItemTypeID;
            target.AreaID = areaId;
            target.Title = source.Title;
            target.Capacity = source.Capacity;
            target.NumberOfBeds = source.NumberOfBeds;
            target.NumberOfBedrooms = source.NumberOfBedrooms;
            target.NumberOfBathrooms = source.NumberOfBathrooms;
            target.ExactAddress = source.ExactAddress;
            target.ApproximateAddress = source.ApproximateAddress;
            target.Description = source.Description;
            target.HostRules = source.HostRules;
            target.MinimumNights = source.MinimumNights;
            target.MaximumNights = source.MaximumNights;
        }

        private static long ResolveAreaId(Seoul_StayDataContext db, string approximateAddress, long fallbackAreaId)
        {
            var value = (approximateAddress ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallbackAreaId;
            }

            var areas = db.Areas.ToList();
            var exact = areas.FirstOrDefault(a =>
                string.Equals(a.Name, value, StringComparison.OrdinalIgnoreCase));

            if (exact != null)
            {
                return exact.ID;
            }

            var contains = areas.FirstOrDefault(a =>
                value.IndexOf(a.Name, StringComparison.OrdinalIgnoreCase) >= 0 ||
                a.Name.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0);

            return contains != null ? contains.ID : fallbackAreaId;
        }

        private static void ReplaceItemAmenities(Seoul_StayDataContext db, long itemId, IEnumerable<long> amenityIds)
        {
            var newIds = (amenityIds ?? Enumerable.Empty<long>()).Distinct().ToList();

            var current = db.ItemAmenities
                                 .Where(x => x.ItemID == itemId)
                                 .ToList();

            var currentIds = current.Select(x => x.AmenityID).ToList();

            // Xóa những cái không còn
            var toRemove = current.Where(x => !newIds.Contains(x.AmenityID));
            db.ItemAmenities.DeleteAllOnSubmit(toRemove);

            // Thêm cái mới
            var toAdd = newIds.Where(id => !currentIds.Contains(id));
            foreach (var id in toAdd)
            {
                db.ItemAmenities.InsertOnSubmit(new ItemAmenity
                {
                    GUID = Guid.NewGuid(),
                    ItemID = itemId,
                    AmenityID = id
                });
            }
        }

        private static void ReplaceItemAttractions(Seoul_StayDataContext db, long itemId, IEnumerable<DTO_Attraction_Distance> attractions)
        {
            var newList = (attractions ?? Enumerable.Empty<DTO_Attraction_Distance>())
        .Where(x => x.AttractionID > 0 && x.Distance.HasValue)
        .GroupBy(x => x.AttractionID)
        .Select(g => g.First())
        .ToList();

            var current = db.ItemAttractions
                                 .Where(x => x.ItemID == itemId)
                                 .ToList();

            var currentIds = current.Select(x => x.AttractionID).ToList();
            var newIds = newList.Select(x => x.AttractionID).ToList();

            // Xóa cái không còn
            var toRemove = current.Where(x => !newIds.Contains(x.AttractionID));
            db.ItemAttractions.DeleteAllOnSubmit(toRemove);

            // Update cái có sẵn
            foreach (var exist in current.Where(x => newIds.Contains(x.AttractionID)))
            {
                var newData = newList.First(x => x.AttractionID == exist.AttractionID);

                exist.Distance = newData.Distance;
                exist.DurationOnFoot = ToNullableInt(newData.OnFoot);
                exist.DurationByCar = ToNullableInt(newData.ByCar);
            }

            // Thêm cái mới
            var toAdd = newList.Where(x => !currentIds.Contains(x.AttractionID));
            foreach (var a in toAdd)
            {
                db.ItemAttractions.InsertOnSubmit(new ItemAttraction
                {
                    GUID = Guid.NewGuid(),
                    ItemID = itemId,
                    AttractionID = a.AttractionID,
                    Distance = a.Distance,
                    DurationOnFoot = ToNullableInt(a.OnFoot),
                    DurationByCar = ToNullableInt(a.ByCar)
                });
            }
        }

        private static int? ToNullableInt(long? value)
        {
            return value.HasValue ? (int?)Convert.ToInt32(value.Value) : null;
        }
        public bool Delete(long id)
        {
            using (var db = new Seoul_StayDataContext())
            {
                try
                {
                    var item = db.Items.FirstOrDefault(i => i.ID == id);
                    if (item == null) return false;

                    item.IsActive = false; // soft delete
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    return false;
                }

            }
        }
    //    public bool ItemHasAmenities(
    //long itemId,
    //List<long> amenityIds)
    //    {
    //        using (var db = new Seoul_StayDataContext())
    //        {
    //            return amenityIds.All(id =>
    //                db.ItemAmenities.Any(x =>
    //                    x.ItemID == itemId
    //                    && x.AmenityID == id));
    //        }
    //    }
        public List<long> GetItemIdsByAmenities(
    List<long> amenityIds)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.ItemAmenities
                    .Where(x =>
                        amenityIds.Contains(x.AmenityID))
                    .GroupBy(x => x.ItemID)
                    .Where(g =>
                        amenityIds.All(id =>
                            g.Any(x =>
                                x.AmenityID == id)))
                    .Select(g => g.Key)
                    .ToList();
            }
        }
        public List<long> GetCurrentlyAvailableItemIds()
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.ItemAvailabilities.Where(x => x.IsAvailable).Select(x => x.ItemID).Distinct().ToList();
            }
        }
    }
}
