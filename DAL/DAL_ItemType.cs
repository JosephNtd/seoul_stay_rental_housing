using ET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DAL
{
    public class DAL_ItemType
    {
        // 1. LẤY DỮ LIỆU
        public List<ET_ItemTypes> GetData()
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data = db.ItemTypes
                    .Select(t => new
                    {
                        t.ID,
                        t.GUID,
                        t.Name,
                        t.Description,
                        t.Icon
                    })
                    .ToList();

                return data.Select(t => new ET_ItemTypes
                {
                    ID = t.ID,
                    GUID = t.GUID,
                    Name = t.Name,
                    Description = t.Description,

                    IconPath = !string.IsNullOrEmpty(t.Icon)
                        ? Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "Images",
                            t.Icon)
                        : null
                }).ToList();
            }
        }

        // 2. KIỂM TRA TRÙNG TÊN LOẠI
        public bool IsNameExists(string name, long idToIgnore = 0)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.ItemTypes.Any(x => (x.Name ?? "").ToLower() == (name ?? "").ToLower() && x.ID != idToIgnore);
            }
        }

        // 3. THÊM MỚI
        public bool Insert(ET_ItemTypes et, string iconFileName)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var itemType = new ItemType
                    {
                        GUID = Guid.NewGuid(),
                        Name = et.Name,
                        Description = et.Description,
                        Icon = iconFileName // Chỉ lưu tên file vào Database
                    };
                    db.ItemTypes.InsertOnSubmit(itemType);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch { return false; }
        }

        // 4. SỬA
        public bool Update(ET_ItemTypes et, string iconFileName)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var itemType = db.ItemTypes.FirstOrDefault(x => x.ID == et.ID);
                    if (itemType != null)
                    {
                        itemType.Name = et.Name;
                        itemType.Description = et.Description;

                        // Nếu user có chọn ảnh mới thì mới update lại tên file Icon
                        if (!string.IsNullOrEmpty(iconFileName))
                        {
                            itemType.Icon = iconFileName;
                        }

                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch { return false; }
        }

        // 5. XÓA
        public bool Delete(long id)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    // KIỂM TRA KHÓA NGOẠI: Có Item nào đang dùng loại này không?
                    if (db.Items.Any(i => i.ItemTypeID == id))
                    {
                        return false; // Trả về false để báo lỗi bên GUI
                    }

                    var itemType = db.ItemTypes.FirstOrDefault(x => x.ID == id);
                    if (itemType != null)
                    {
                        db.ItemTypes.DeleteOnSubmit(itemType);
                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch { return false; }
        }
        public ET_ItemTypes GetByID(long id)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var t = db.ItemTypes
                    .FirstOrDefault(x => x.ID == id);

                if (t == null)
                    return null;

                return new ET_ItemTypes
                {
                    ID = t.ID,
                    GUID = t.GUID,
                    Name = t.Name,
                    Description = t.Description,

                    IconPath = !string.IsNullOrEmpty(t.Icon)
                        ? Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "Images",
                            t.Icon)
                        : null
                };
            }
        }
        public List<ET_Items> GetItemByType(long itemTypeID)
        {
            using(var db = new Seoul_StayDataContext())
            {
                return db.Items
                        .Where(x => x.ItemTypeID == itemTypeID)
                        .Select(x => new ET_Items
                        {
                            ID = x.ID,
                            GUID = x.GUID,
                            HostUserID = x.HostUserID,
                            ItemTypeID = x.ItemTypeID,
                            AreaID = x.AreaID,
                            Title = x.Title,
                            Capacity = x.Capacity,
                            NumberOfBeds = x.NumberOfBeds,
                            NumberOfBedrooms = x.NumberOfBedrooms,
                            NumberOfBathrooms = x.NumberOfBathrooms,
                            ExactAddress = x.ExactAddress,
                            ApproximateAddress = x.ApproximateAddress,
                            Description = x.Description,
                            HostRules = x.HostRules,
                            MinimumNights = x.MinimumNights,
                            MaximumNights = x.MaximumNights,
                            IsCreated = x.CreatedDate,
                            IsActive = x.IsActive ? (byte)1 : (byte)0
                        })
                        .ToList();
            }
        }
    }
}