using ET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_ServiceType
    {
        public List<ET_ServiceType> GetData()
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data = db.ServiceTypes
                    .Select(t => new
                    {
                        t.ID,
                        t.GUID,
                        t.Name,
                        t.Description,
                        t.IconName
                    })
                    .ToList();

                return data.Select(t => new ET_ServiceType
                {
                    ID = t.ID,
                    GUID = t.GUID,
                    Name = t.Name,
                    Description = t.Description,

                    IconPath = !string.IsNullOrEmpty(t.IconName)
                        ? Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "Images",
                            t.IconName)
                        : null
                }).ToList();
            }
        }
        // 2. KIỂM TRA TRÙNG TÊN LOẠI
        public bool IsNameExists(string name, long idToIgnore = 0)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.ServiceTypes.Any(x => x.Name.ToLower() == name.ToLower() && x.ID != idToIgnore);
            }
        }

        // 3. THÊM MỚI
        public bool Insert(ET_ServiceType et, string iconFileName)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var ServiceType = new ServiceType
                    {
                        GUID = Guid.NewGuid(),
                        Name = et.Name,
                        Description = et.Description,
                        IconName = iconFileName // Chỉ lưu tên file vào Database
                    };
                    db.ServiceTypes.InsertOnSubmit(ServiceType);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch { return false; }
        }

        // 4. SỬA
        public bool Update(ET_ServiceType et, string iconFileName)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var ServiceType = db.ServiceTypes.FirstOrDefault(x => x.ID == et.ID);
                    if (ServiceType != null)
                    {
                        ServiceType.Name = et.Name;
                        ServiceType.Description = et.Description;

                        // Nếu user có chọn ảnh mới thì mới update lại tên file Icon
                        if (!string.IsNullOrEmpty(iconFileName))
                        {
                            ServiceType.IconName = iconFileName;
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
                    if (db.Services.Any(i => i.ServiceTypeID == id))
                    {
                        return false; // Trả về false để báo lỗi bên GUI
                    }

                    var ServiceType = db.ServiceTypes.FirstOrDefault(x => x.ID == id);
                    if (ServiceType != null)
                    {
                        db.ServiceTypes.DeleteOnSubmit(ServiceType);
                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch { return false; }
        }
        // =========================================
        // GET BY ID
        // =========================================

        public ET_ServiceType GetByID(long id)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var t = db.ServiceTypes
                    .FirstOrDefault(x => x.ID == id);

                if (t == null)
                    return null;

                return new ET_ServiceType
                {
                    ID = t.ID,
                    GUID = t.GUID,
                    Name = t.Name,
                    Description = t.Description,

                    IconPath = !string.IsNullOrEmpty(t.IconName)
                        ? Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "Images",
                            t.IconName)
                        : null
                };
            }
        }

    }
}
