using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class ET_Coupons
    {


        public long ID { get; set; }
        public Guid GUID { get; set; }
        public string CouponCode { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MaximumDiscountAmount { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int CurrentUsageCount { get; set; }
        public bool IsActive { get; set; }
        public string UsageText
        {
            get => $"{CurrentUsageCount}/ {MaxUsageCount}";
        }

        // =============================================
        // DISPLAY HELPERS FOR MANAGEMENT SCREEN
        // =============================================
        public string StatusText
        {
            get
            {
                if (!IsActive)
                    return "Inactive";

                if (DateTime.Today < StartedDate.Date)
                    return "Upcoming";

                if (ExpirationDate.HasValue && DateTime.Today > ExpirationDate.Value.Date)
                    return "Expired";

                if (MaxUsageCount.HasValue && CurrentUsageCount >= MaxUsageCount.Value)
                    return "Used Up";

                return "Active";
            }
        }

        public string UsageDisplay
        {
            get
            {
                if (MaxUsageCount.HasValue)
                    return $"{CurrentUsageCount} / {MaxUsageCount.Value}";
                return $"{CurrentUsageCount} / ∞";
            }
        }

        public string StartedDateDisplay => StartedDate.ToString("dd/MM/yyyy");

        public string ExpirationDisplay => ExpirationDate.HasValue
            ? ExpirationDate.Value.ToString("dd/MM/yyyy")
            : "Never";

        public string MaxDiscountDisplay => $"{MaximumDiscountAmount:N0} ₫";

        public string DiscountPercentDisplay => $"{DiscountPercent:N0}%";
        public ET_Coupons() { }

        public ET_Coupons(long iD, Guid gUID, string couponCode, decimal discountPercent, decimal maximumDiscountAmount, DateTime startedDate, DateTime expirationDate, int maxUsageCount, int currentUsageCount, bool isActive)
        {
            ID = iD;
            GUID = gUID;
            CouponCode = couponCode;
            DiscountPercent = discountPercent;
            MaximumDiscountAmount = maximumDiscountAmount;
            StartedDate = startedDate;
            ExpirationDate = expirationDate;
            MaxUsageCount = maxUsageCount;
            CurrentUsageCount = currentUsageCount;
            IsActive = isActive;
        }
    }
}