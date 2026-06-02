using DAL;
using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUS_ServiceType
    {
        private readonly DAL_ServiceType _dal = new DAL_ServiceType();

        public List<ET_ServiceType> GetData() => _dal.GetData();
        public bool IsNameExists(string name, long idToIgnore = 0) => _dal.IsNameExists(name, idToIgnore);

        public bool Insert(ET_ServiceType et, string iconFileName) => _dal.Insert(et, iconFileName);

        public bool Update(ET_ServiceType et, string iconFileName) => _dal.Update(et, iconFileName);

        public bool Delete(long id) => _dal.Delete(id);
        public ET_ServiceType GetByID(long id) => _dal.GetByID(id);
    }
}
