using DAL;
using ET;
using System.Collections.Generic;

namespace BUS
{
    public class BUS_Coupon
    {
        private DAL_Coupon _dal = new DAL_Coupon();

        public List<ET_Coupons> GetAll() => _dal.GetData();

        public bool IsCodeExists(string code, long idToIgnore = 0) => _dal.IsCodeExists(code, idToIgnore);

        public bool Insert(ET_Coupons et) => _dal.Insert(et);

        public bool Update(ET_Coupons et) => _dal.Update(et);

        public bool Delete(long id) => _dal.Delete(id);
    }
}