using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_UI.Models;

[Index("Guid", Name = "UQ_Bookings_GUID", IsUnique = true)]
public partial class Booking
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("GUID")]
    public Guid Guid { get; set; }

    [Column("GuestUserID")]
    public long GuestUserId { get; set; }

    [Column("ItemID")]
    public long ItemId { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public int NumberOfGuests { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PricePerNight { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalPrice { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal FinalPrice { get; set; }

    [Column("CancellationPolicyID")]
    public long CancellationPolicyId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string BookingStatus { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime BookingDate { get; set; }

    [StringLength(1000)]
    public string? SpecialRequests { get; set; }

    [Column("TransactionID")]
    public long? TransactionId { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<AddonService> AddonServices { get; set; } = new List<AddonService>();

    [InverseProperty("Booking")]
    public virtual ICollection<BookingCoupon> BookingCoupons { get; set; } = new List<BookingCoupon>();

    [InverseProperty("Booking")]
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    [InverseProperty("Booking")]
    public virtual ICollection<BookingStatusHistory> BookingStatusHistories { get; set; } = new List<BookingStatusHistory>();

    [ForeignKey("CancellationPolicyId")]
    [InverseProperty("Bookings")]
    public virtual CancellationPolicy CancellationPolicy { get; set; } = null!;

    [ForeignKey("CheckInDate")]
    [InverseProperty("BookingCheckInDateNavigations")]
    public virtual DimDate CheckInDateNavigation { get; set; } = null!;

    [ForeignKey("CheckOutDate")]
    [InverseProperty("BookingCheckOutDateNavigations")]
    public virtual DimDate CheckOutDateNavigation { get; set; } = null!;

    [ForeignKey("GuestUserId")]
    [InverseProperty("Bookings")]
    public virtual Guest GuestUser { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("Bookings")]
    public virtual Item Item { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("TransactionId")]
    [InverseProperty("Bookings")]
    public virtual Transaction? Transaction { get; set; }
}
