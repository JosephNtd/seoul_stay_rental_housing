using DAL;
using ET;
using System;
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

        public ET_Coupons GetByCode(string code) => _dal.GetByCode(code);

        public bool IncrementUsage(long couponId) => _dal.IncrementUsage(couponId);

        /// <summary>
        /// Validate coupon code. Returns null if valid, error message if invalid.
        /// Out parameter returns the coupon entity when valid.
        /// </summary>
        public string ValidateCoupon(string code, out ET_Coupons coupon)
        {
            coupon = null;

            if (string.IsNullOrWhiteSpace(code))
                return "Please enter a coupon code.";

            coupon = _dal.GetByCode(code.Trim());

            if (coupon == null)
                return "Coupon code not found.";

            if (!coupon.IsActive)
                return "This coupon is no longer active.";

            if (DateTime.Today < coupon.StartedDate.Date)
                return $"This coupon is not valid until {coupon.StartedDate:dd/MM/yyyy}.";

            if (coupon.ExpirationDate.HasValue && DateTime.Today > coupon.ExpirationDate.Value.Date)
                return $"This coupon expired on {coupon.ExpirationDate.Value:dd/MM/yyyy}.";

            if (coupon.MaxUsageCount.HasValue && coupon.CurrentUsageCount >= coupon.MaxUsageCount.Value)
                return "This coupon has reached its maximum usage limit.";

            return null;
        }

        /// <summary>
        /// Calculate discount amount based on coupon rules.
        /// Discount = Min(baseAmount * DiscountPercent / 100, MaximumDiscountAmount)
        /// </summary>
        public decimal CalculateDiscount(decimal baseAmount, ET_Coupons coupon)
        {
            if (coupon == null || baseAmount <= 0)
                return 0;

            decimal rawDiscount = baseAmount * coupon.DiscountPercent / 100m;

            if (coupon.MaximumDiscountAmount > 0 && rawDiscount > coupon.MaximumDiscountAmount)
                rawDiscount = coupon.MaximumDiscountAmount;

            return Math.Round(rawDiscount, 2);
        }
    }
}