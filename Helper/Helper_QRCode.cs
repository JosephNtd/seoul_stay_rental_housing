using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Helper
{
    public class Helper_QRCode
    {
        /// <summary>
        /// Generate QR Code dưới dạng BitmapImage (WPF-compatible) từ URL string.
        /// </summary>
        /// <param name="url">URL nhúng vào QR (ví dụ: http://192.168.1.5:5000/payment/42)</param>
        /// <param name="pixelsPerModule">Kích thước pixel mỗi module QR (mặc định 10)</param>
        /// <returns>BitmapImage hiển thị được trong WPF Image control</returns>
        public static BitmapImage GenerateBitmapImage(string url, int pixelsPerModule = 10)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap bitmap = qrCode.GetGraphic(pixelsPerModule))
                    {
                        return BitmapToBitmapImage(bitmap);
                    }
                }
            }
        }

        /// <summary>
        /// Generate QR Code dưới dạng byte[] (PNG) — dùng để nhúng vào PDF.
        /// </summary>
        /// <param name="url">URL nhúng vào QR</param>
        /// <param name="pixelsPerModule">Kích thước pixel mỗi module QR (mặc định 10)</param>
        /// <returns>Byte array chứa PNG image</returns>
        public static byte[] GeneratePngBytes(string url, int pixelsPerModule = 10)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap bitmap = qrCode.GetGraphic(pixelsPerModule))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitmap.Save(ms, ImageFormat.Png);
                            return ms.ToArray();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert System.Drawing.Bitmap → WPF BitmapImage
        /// </summary>
        private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}