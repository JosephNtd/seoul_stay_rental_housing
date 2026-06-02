using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Services
    {
        public List<ET_Services> GetData()
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data = from s in db.Services
                           join st in db.ServiceTypes on s.ServiceTypeID equals st.ID
                           select new ET_Services
                           {
                               ID = s.ID,
                               GUID = s.GUID,
                               ServiceTypeID = s.ServiceTypeID,
                               ServiceTypeName = st.Name,
                               Name = s.Name,
                               Price = s.Price,
                               Duration = s.Duration,
                               Description = s.Description,
                               DayOfWeek = s.DayOfWeek,
                               DayOfMonth = s.DayOfMonth,
                               DailyCap = s.DailyCap,
                               BookingCap = s.BookingCap
                           };
                return data.ToList();
            }
        }

        public bool IsNameExists(string name, long idToIgnore = 0)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Services.Any(x => x.Name.ToLower() == name.ToLower() && x.ID != idToIgnore);
            }
        }

        public bool Insert(ET_Services et)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var srv = new Service
                    {
                        GUID = Guid.NewGuid(),
                        ServiceTypeID = et.ServiceTypeID,
                        Name = et.Name,
                        Price = et.Price,
                        Duration = et.Duration,
                        Description = et.Description,
                        DayOfWeek = et.DayOfWeek,
                        DayOfMonth = et.DayOfMonth,
                        DailyCap = et.DailyCap,
                        BookingCap = et.BookingCap
                    };
                    db.Services.InsertOnSubmit(srv);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch { return false; }
        }

        public bool Update(ET_Services et)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var srv = db.Services.FirstOrDefault(x => x.ID == et.ID);
                    if (srv != null)
                    {
                        srv.ServiceTypeID = et.ServiceTypeID;
                        srv.Name = et.Name;
                        srv.Price = et.Price;
                        srv.Duration = et.Duration;
                        srv.Description = et.Description;
                        srv.DayOfWeek = et.DayOfWeek;
                        srv.DayOfMonth = et.DayOfMonth;
                        srv.DailyCap = et.DailyCap;
                        srv.BookingCap = et.BookingCap;

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
                    var srv = db.Services.FirstOrDefault(x => x.ID == id);
                    if (srv != null)
                    {
                        db.Services.DeleteOnSubmit(srv);
                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch { return false; }
        }
        // GET BY ID

        public ET_Services GetByID(long id)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data =
                    (from s in db.Services
                     join st in db.ServiceTypes
                     on s.ServiceTypeID equals st.ID
                     where s.ID == id
                     select new ET_Services
                     {
                         ID = s.ID,
                         GUID = s.GUID,
                         ServiceTypeID = s.ServiceTypeID,
                         ServiceTypeName = st.Name,
                         Name = s.Name,
                         Price = s.Price,
                         Duration = s.Duration,
                         Description = s.Description,
                         DayOfWeek = s.DayOfWeek,
                         DayOfMonth = s.DayOfMonth,
                         DailyCap = s.DailyCap,
                         BookingCap = s.BookingCap
                     })
                     .FirstOrDefault();

                return data;
            }
        }

        // GET BY TYPE

        public List<ET_Services> GetByType(long typeId)
        {
            return GetData()
                .Where(x => x.ServiceTypeID == typeId)
                .ToList();
        }

    }
}
