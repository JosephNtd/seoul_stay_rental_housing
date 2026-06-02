using DTO;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Area
    {

        public List<object> GetHotelCountByArea()
        {
            using (Seoul_StayDataContext db = new Seoul_StayDataContext())
            {
                var result = from a in db.Areas
                             join i in db.Items on a.ID equals i.AreaID into g
                             from x in g.DefaultIfEmpty()
                             group x by a.Name into grp
                             select new
                             {
                                 AreaName = grp.Key,
                                 HotelCount = grp.Count(i => i != null)
                             };

                return result.ToList<object>();
            }
        }

        public List<ET_Areas> GetAreaName()
        {
            using (Seoul_StayDataContext db = new Seoul_StayDataContext())
            {
                var data = from t in db.Areas
                           select new ET_Areas
                           {
                               ID = t.ID,
                               Name = t.Name
                           };
                return data.ToList();
            }
        }

        public List<DTO_AreaDisplay> GetData()
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data = (from a in db.Areas
                            join i in db.Items on a.ID equals i.AreaID into g
                            from x in g.DefaultIfEmpty()
                            group x by new
                            {
                                a.ID,
                                a.Name
                            } into grp

                            select new DTO_AreaDisplay
                            {
                                AreaID = grp.Key.ID,
                                AreaName = grp.Key.Name,
                                StayCount = grp.Count(x => x != null)
                            }).ToList();
                return data;
            }

        }

        public DTO_AreaOverview GetAreaOverview(long areaId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                int totalItems = db.Items.Count(i => i.AreaID == areaId);

                var totalAmenities = (from i in db.Items
                                      join ia in db.ItemAmenities on i.ID equals ia.ItemID
                                      where i.AreaID == areaId
                                      select ia.AmenityID).Distinct().Count();

                // Trả về object DTO public thay vì Anonymous Type
                return new DTO_AreaOverview
                {
                    TotalItems = totalItems,
                    TotalAmenities = totalAmenities
                };
            }
        }

        public List<DTO_AreaItems> GetItemsByArea(long areaId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var items = db.Items.Where(i => i.AreaID == areaId)
                                    .Select(i => new DTO_AreaItems
                                    {
                                        ID = i.ID,
                                        Name = i.Title,
                                        Capacity = i.Capacity,
                                        TypeName = i.ItemType.Name,
                                        ApproximateAddress = i.ApproximateAddress
                                    }).ToList();
                return items;
            }
        }

        public object GetAttractionsByArea(long areaId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var attractions = db.Attractions.Where(a => a.AreaID == areaId)
                                                .Select(a => new
                                                {
                                                    a.ID,
                                                    TênĐịaDanh = a.Name,
                                                    Địa_Chỉ = a.Address
                                                }).ToList();
                return attractions;
            }
        }
        // =========================================
        // GET BY ID
        // =========================================
        public ET_Areas GetByID(long id)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var a = db.Areas.FirstOrDefault(x => x.ID == id);

                if (a == null)
                    return null;

                return new ET_Areas
                {
                    ID = a.ID,
                    GUID = a.GUID,
                    Name = a.Name
                };
            }
                
        }

        // =========================================
        // INSERT
        // =========================================
        public bool Insert(ET_Areas et)
        {
            using (var db = new Seoul_StayDataContext())
            {
                try
                {
                    Area a = new Area
                    {
                        GUID = Guid.NewGuid(),
                        Name = et.Name
                    };

                    db.Areas.InsertOnSubmit(a);

                    db.SubmitChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            } 
        }

        // =========================================
        // UPDATE
        // =========================================
        public bool Update(ET_Areas et)
        {
            using (var db = new Seoul_StayDataContext())
            {
                try
                {
                    var a = db.Areas.FirstOrDefault(x => x.ID == et.ID);

                    if (a == null)
                        return false;

                    a.Name = et.Name;

                    db.SubmitChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // =========================================
        // DELETE
        // =========================================
        public bool Delete(long id)
        {
            using (var db = new Seoul_StayDataContext())
            {
                try
                {
                    var area = db.Areas.FirstOrDefault(x => x.ID == id);

                    if (area == null)
                        return false;

                    bool hasItems =
                        db.Items.Any(i => i.AreaID == id);

                    if (hasItems)
                        return false;

                    db.Areas.DeleteOnSubmit(area);

                    db.SubmitChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // =========================================
        // CHECK NAME
        // =========================================
        public bool IsNameExists(string name, long idToIgnore = 0)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Areas.Any(x =>
               x.Name.ToLower() == name.ToLower()
               && x.ID != idToIgnore);
            }
        }
    }
}
