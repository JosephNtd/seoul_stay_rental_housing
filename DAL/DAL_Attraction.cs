using DTO;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Attraction
    {
        private readonly Seoul_StayDataContext db = new Seoul_StayDataContext();
        public List<DTO_Attraction_Distance> GetData(long? itemId = null)
        {
            var itemAttractions = itemId.HasValue
                ? db.ItemAttractions.Where(i => i.ItemID == itemId.Value)
                : db.ItemAttractions.Where(i => false);

            return (from a in db.Attractions
                    join b in db.Areas on a.AreaID equals b.ID
                    join i in itemAttractions on a.ID equals i.AttractionID into joined
                    from i in joined.DefaultIfEmpty()
                    select new DTO_Attraction_Distance
                    {
                        AttractionID = a.ID,
                        Attraction = a.Name,
                        Area = b.Name,
                        Distance = i == null ? (decimal?)null : i.Distance,
                        OnFoot = i == null ? (long?)null : i.DurationOnFoot,
                        ByCar = i == null ? (long?)null : i.DurationByCar,
                    }).ToList();
        }
        public List<ET_Attractions> GetData()
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Attractions.Select(a => new ET_Attractions
                {
                    ID = a.ID,
                    GUID = a.GUID,
                    AreaID = a.AreaID,
                    AttractionName = a.Name,
                    Address = a.Address
                }).ToList();
            }
        }
        public List<DTO_AttractionDisplay> GetAllWithAreaName()
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data = from attr in db.Attractions
                           join ar in db.Areas on attr.AreaID equals ar.ID
                           select new DTO_AttractionDisplay
                           {
                               ID = attr.ID,
                               GUID = attr.GUID,
                               AttractionName = attr.Name,
                               Address = attr.Address,
                               AreaID = attr.AreaID,
                               AreaName = ar.Name // Lấy tên vùng từ bảng Areas
                           };
                return data.ToList();
            }
        }
        public bool Insert(ET_Attractions et)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var attr = new Attraction
                    {
                        GUID = Guid.NewGuid(), // Tự động sinh GUID mới
                        AreaID = et.AreaID,
                        Name = et.AttractionName,
                        Address = et.Address
                    };
                    db.Attractions.InsertOnSubmit(attr);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch
            {
                
                return false;
            }
        }
        // Kiểm tra tên đã tồn tại chưa. 
        // idToIgnore: Nếu đang Sửa (Edit), ta phải bỏ qua ID của chính nó để nó không tự báo trùng với tên cũ của nó.
        public bool IsNameExists(string name, long idToIgnore = 0)
        {
            using (var db = new Seoul_StayDataContext())
            {
                // ToLower() để không phân biệt hoa/thường (ví dụ "Namsan" và "namsan" là giống nhau)
                return db.Attractions.Any(x => x.Name.ToLower() == name.ToLower() && x.ID != idToIgnore);
            }
        }

        public bool Update(ET_Attractions et)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    // Tìm bản ghi cần sửa theo ID
                    var attr = db.Attractions.FirstOrDefault(x => x.ID == et.ID);
                    if (attr != null)
                    {
                        attr.AreaID = et.AreaID;
                        attr.Name = et.AttractionName;
                        attr.Address = et.Address;

                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch { return false; }
        }

        public bool Delete(long id)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    // Xóa các bản ghi phụ thuộc trong bảng ItemAttractions 
                    var itemAttrs = db.ItemAttractions.Where(x => x.AttractionID == id);
                    db.ItemAttractions.DeleteAllOnSubmit(itemAttrs);

                    // Xóa bản ghi chính
                    var attr = db.Attractions.FirstOrDefault(x => x.ID == id);
                    if (attr != null)
                    {
                        db.Attractions.DeleteOnSubmit(attr);
                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch { return false; }
        }
    }
}
