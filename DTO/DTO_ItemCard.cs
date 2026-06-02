using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DTO
{
    public class DTO_ItemCard
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string ApproximateAddress { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public string AreaName { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfBedrooms { get; set; }
        public int NumberOfBathrooms { get; set; }
        public decimal? MinPrice { get; set; }
        public string ThumbnailPath { get; set; }
        public string Status { get; set; } // "Active", "Occupied", "Draft"
        public string HostName { get; set; } // tùy chọn
        public string FullThumbnailPath => string.IsNullOrEmpty(ThumbnailPath)
        ? null
        : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Listings", ThumbnailPath);

        private Image _imageDisplay;
        // ET_ItemPictures.cs
        public Image ImageDisplay
        {
            get
            {
                // Nếu đã load ảnh rồi thì trả về luôn, không cần đọc lại từ ổ cứng
                if (_imageDisplay != null) return _imageDisplay;

                if (string.IsNullOrEmpty(FullThumbnailPath) || !File.Exists(FullThumbnailPath))
                    return null;

                try
                {
                    // Dùng MemoryStream để đọc ảnh không bị lock file
                    byte[] bytes = File.ReadAllBytes(FullThumbnailPath);
                    _imageDisplay = Image.FromStream(new MemoryStream(bytes));
                    return _imageDisplay;
                }
                catch
                {
                    return null;
                }
            }
        }

    }
}
