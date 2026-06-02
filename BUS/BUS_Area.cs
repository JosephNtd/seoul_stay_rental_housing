using DAL;
using DTO;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUS_Area
    {
        DAL_Area dal = new DAL_Area();

        public List<object> GetThongKe() => dal.GetHotelCountByArea();
        public List<ET_Areas> GetAreasName() => dal.GetAreaName();
        public List<DTO_AreaDisplay> GetData() => dal.GetData();
        public DTO_AreaOverview GetAreaOverview(long areaId) => dal.GetAreaOverview(areaId);
        public List<DTO_AreaItems> GetItemsByArea(long areaId) => dal.GetItemsByArea(areaId);
        public object GetAttractionsByArea(long areaId) => dal.GetAttractionsByArea(areaId);
        // =========================================
        // GET BY ID
        // =========================================
        public ET_Areas GetByID(long id)
        {
            return dal.GetByID(id);
        }

        // =========================================
        // INSERT
        // =========================================
        public bool Insert(ET_Areas area)
        {
            return dal.Insert(area);
        }

        // =========================================
        // UPDATE
        // =========================================
        public bool Update(ET_Areas area)
        {
            return dal.Update(area);
        }

        // =========================================
        // DELETE
        // =========================================
        public bool Delete(long id)
        {
            return dal.Delete(id);
        }

        // =========================================
        // VALIDATION
        // =========================================
        public bool IsNameExists(string name, long idToIgnore = 0)
        {
            return dal.IsNameExists(name, idToIgnore);
        }

    }
}
