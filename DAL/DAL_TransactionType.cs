using ET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DAL
{
    public class DAL_TransactionType
    {
        // 1. LẤY DỮ LIỆU
        public List<ET_TransactionTypes> GetData()
        {
            using (var db = new Seoul_StayDataContext())
            {
                var data = db.TransactionTypes
                    .Select(t => new
                    {
                        t.ID,
                        t.GUID,
                        t.Name
                    })
                    .ToList();

                return data.Select(t => new ET_TransactionTypes
                {
                    ID = t.ID,
                    GUID = t.GUID,
                    Name = t.Name,

                }).ToList();
            }
        }

        // 2. KIỂM TRA TRÙNG TÊN LOẠI
        public bool IsNameExists(string name, long idToIgnore = 0)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.TransactionTypes.Any(x => x.Name.ToLower() == name.ToLower() && x.ID != idToIgnore);
            }
        }

        // 3. THÊM MỚI
        public bool Insert(ET_TransactionTypes et)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var itemType = new TransactionType
                    {
                        GUID = Guid.NewGuid(),
                        Name = et.Name,
                    };
                    db.TransactionTypes.InsertOnSubmit(itemType);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch { return false; }
        }

        // 4. SỬA
        public bool Update(ET_TransactionTypes et)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var itemType = db.TransactionTypes.FirstOrDefault(x => x.ID == et.ID);
                    if (itemType != null)
                    {
                        itemType.Name = et.Name;

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
                    if (db.Transactions.Any(t => t.TransactionTypeID == id))
                        return false;

                    var itemType = db.TransactionTypes.FirstOrDefault(x => x.ID == id);
                    if (itemType != null)
                    {
                        db.TransactionTypes.DeleteOnSubmit(itemType);
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