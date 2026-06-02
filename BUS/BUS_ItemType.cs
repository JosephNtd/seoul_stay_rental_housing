using DAL;
using ET;
using System;
using System.Collections.Generic;

namespace BUS
{
    public class BUS_ItemType
    {
        private readonly DAL_ItemType _dal = new DAL_ItemType();

        public List<ET_ItemTypes> GetData() => _dal.GetData();

        public bool IsNameExists(string name, long idToIgnore = 0) => _dal.IsNameExists(name, idToIgnore);

        public bool Insert(ET_ItemTypes et, string iconFileName) => _dal.Insert(et, iconFileName);

        public bool Update(ET_ItemTypes et, string iconFileName) => _dal.Update(et, iconFileName);

        public bool Delete(long id) => _dal.Delete(id);
        public ET_ItemTypes GetByID(long id) => _dal.GetByID(id);
        public List<ET_Items> GetItemsByItemTypeID(long itemTypeID) => _dal.GetItemByType(itemTypeID);
    }
}