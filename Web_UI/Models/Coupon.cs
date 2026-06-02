using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_UI.Models;

[Index("CouponCode", Name = "UQ_Coupons_CouponCode", IsUnique = true)]
[Index("Guid", Name = "UQ_Coupons_GUID", IsUnique = true)]
public partial class Coupon
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("GUID")]
    public Guid Guid { get; set; }

    [StringLength(50)]
    public string CouponCode { get; set; } = null!;

    [Column(TypeName = "decimal(4, 1)")]
    public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal MaximumDiscountAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? MaxUsageCount { get; set; }

    public int CurrentUsageCount { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Coupon")]
    public virtual ICollection<AddonService> AddonServices { get; set; } = new List<AddonService>();

    [InverseProperty("Coupon")]
    public virtual ICollection<BookingCoupon> BookingCoupons { get; set; } = new List<BookingCoupon>();
}
