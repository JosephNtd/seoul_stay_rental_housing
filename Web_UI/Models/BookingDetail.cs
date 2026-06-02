using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_UI.Models;

[Index("Guid", Name = "UQ_BookingDetails_GUID", IsUnique = true)]
public partial class BookingDetail
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("GUID")]
    public Guid Guid { get; set; }

    [Column("BookingID")]
    public long BookingId { get; set; }

    [Column("ItemPriceID")]
    public long ItemPriceId { get; set; }

    [Column("isRefund")]
    public bool IsRefund { get; set; }

    public DateTime? RefundDate { get; set; }

    [Column("RefundCancellationPolicyID")]
    public long? RefundCancellationPolicyId { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("BookingDetails")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("ItemPriceId")]
    [InverseProperty("BookingDetails")]
    public virtual ItemPrice ItemPrice { get; set; } = null!;

    [ForeignKey("RefundCancellationPolicyId")]
    [InverseProperty("BookingDetails")]
    public virtual CancellationPolicy? RefundCancellationPolicy { get; set; }
}
