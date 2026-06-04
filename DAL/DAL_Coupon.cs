using ET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class DAL_Coupon
    {
        // 1. LẤY DỮ LIỆU
        public List<ET_Coupons> GetData()
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Coupons.Select(c => new ET_Coupons
                {
                    ID = c.ID,
                    GUID = c.GUID,
                    CouponCode = c.CouponCode,
                    DiscountPercent = c.DiscountPercent,
                    MaximumDiscountAmount = c.MaximumDiscountAmount,
                    StartedDate = c.StartDate, // Lưu ý: Trong DB có thể là StartDate
                    ExpirationDate = c.ExpirationDate,
                    MaxUsageCount = c.MaxUsageCount,
                    CurrentUsageCount = c.CurrentUsageCount,
                    IsActive = c.IsActive
                }).ToList();
            }
        }

        // 2. KIỂM TRA TRÙNG MÃ (Bỏ qua ID hiện tại nếu đang ở chế độ Sửa)
        public bool IsCodeExists(string code, long idToIgnore = 0)
        {
            using (var db = new Seoul_StayDataContext())
            {
                return db.Coupons.Any(x => x.CouponCode.ToLower() == code.ToLower() && x.ID != idToIgnore);
            }
        }

        // 3. THÊM MỚI
        public bool Insert(ET_Coupons et)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var coupon = new Coupon
                    {
                        GUID = Guid.NewGuid(),
                        CouponCode = et.CouponCode,
                        DiscountPercent = et.DiscountPercent,
                        MaximumDiscountAmount = et.MaximumDiscountAmount,
                        StartDate = et.StartedDate,
                        ExpirationDate = et.ExpirationDate, // Nhận giá trị null bình thường
                        MaxUsageCount = et.MaxUsageCount,   // Nhận giá trị null bình thường
                        CurrentUsageCount = 0, // Mặc định khi mới tạo thì chưa ai dùng
                        IsActive = et.IsActive
                    };
                    db.Coupons.InsertOnSubmit(coupon);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch { return false; }
        }

        // 4. CẬP NHẬT
        public bool Update(ET_Coupons et)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var coupon = db.Coupons.FirstOrDefault(x => x.ID == et.ID);
                    if (coupon != null)
                    {
                        coupon.CouponCode = et.CouponCode;
                        coupon.DiscountPercent = et.DiscountPercent;
                        coupon.MaximumDiscountAmount = et.MaximumDiscountAmount;
                        coupon.StartDate = et.StartedDate;
                        coupon.ExpirationDate = et.ExpirationDate;
                        coupon.MaxUsageCount = et.MaxUsageCount;
                        coupon.IsActive = et.IsActive;

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
                    var coupon = db.Coupons.FirstOrDefault(x => x.ID == id);
                    if (coupon != null)
                    {
                        db.Coupons.DeleteOnSubmit(coupon);
                        db.SubmitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch { return false; }
        }

        // 6. LẤY COUPON THEO MÃ
        public ET_Coupons GetByCode(string code)
        {
            using (var db = new Seoul_StayDataContext())
            {
                var c = db.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == code.Trim().ToLower());

                if (c == null)
                    return null;

                return new ET_Coupons
                {
                    ID = c.ID,
                    GUID = c.GUID,
                    CouponCode = c.CouponCode,
                    DiscountPercent = c.DiscountPercent,
                    MaximumDiscountAmount = c.MaximumDiscountAmount,
                    StartedDate = c.StartDate,
                    ExpirationDate = c.ExpirationDate,
                    MaxUsageCount = c.MaxUsageCount,
                    CurrentUsageCount = c.CurrentUsageCount,
                    IsActive = c.IsActive
                };
            }
        }

        // 7. TĂNG SỐ LẦN SỬ DỤNG
        public bool IncrementUsage(long couponId)
        {
            try
            {
                using (var db = new Seoul_StayDataContext())
                {
                    var coupon = db.Coupons.FirstOrDefault(x => x.ID == couponId);

                    if (coupon == null)
                        return false;

                    coupon.CurrentUsageCount += 1;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch { return false; }
        }
    }
}