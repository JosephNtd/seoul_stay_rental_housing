using ET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_ItemPictures
    {
        public List<ET_ItemPictures> GetByItem(long itemId)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.ItemPictures
                         .Where(p => p.ItemID == itemId)
                         .OrderBy(p => p.DisplayOrder)
                         .Select(p => new ET_ItemPictures
                         {
                             ID = p.ID,
                             FileName = p.PictureFileName,
                             DisplayOrder = p.DisplayOrder,
                             FullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Listings", p.PictureFileName)
                         })
                         .ToList();
            }
        }

        public void SaveAll(long itemId, List<ET_ItemPictures> pictures)
        {
            using (var db = new Seoul_StayDataContext())
            {
                // Lấy danh sách ID của ảnh mới (đã tồn tại trong DB)
                var existingIds = pictures
                    .Where(p => p.ID > 0)
                    .Select(p => p.ID)
                    .ToArray(); // ⬅️ chuyển thành mảng để hỗ trợ SQL

                // Xóa những ảnh trong DB nhưng không còn trong danh sách mới
                if (existingIds.Length > 0)
                {
                    var toDelete = db.ItemPictures
                        .Where(p => p.ItemID == itemId && !existingIds.Contains(p.ID))
                        .ToList();
                    db.ItemPictures.DeleteAllOnSubmit(toDelete);
                }
                else
                {
                    // Nếu không có ảnh cũ nào giữ lại → xóa tất cả ảnh của item
                    var all = db.ItemPictures.Where(p => p.ItemID == itemId).ToList();
                    db.ItemPictures.DeleteAllOnSubmit(all);
                }

                // Thêm / Cập nhật ảnh
                int order = 0;
                foreach (var pic in pictures)
                {
                    if (pic.ID > 0)
                    {
                        // Cập nhật ảnh đã tồn tại
                        var entity = db.ItemPictures.FirstOrDefault(p => p.ID == pic.ID);
                        if (entity != null)
                        {
                            entity.DisplayOrder = order++;
                            // Nếu có thay đổi tên file hoặc đường dẫn, cập nhật
                            entity.PictureFileName = pic.FileName;
                        }
                    }
                    else
                    {
                        // Thêm ảnh mới
                        var entity = new ItemPicture
                        {
                            GUID = Guid.NewGuid(),
                            ItemID = itemId,
                            PictureFileName = pic.FileName,
                            DisplayOrder = order++
                        };
                        db.ItemPictures.InsertOnSubmit(entity);
                    }
                }

                db.SubmitChanges();
            }
        }
    }
}
