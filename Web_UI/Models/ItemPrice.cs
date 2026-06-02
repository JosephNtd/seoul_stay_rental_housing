using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_UI.Models;

[Index("Guid", Name = "UQ_ItemPrices_GUID", IsUnique = true)]
[Index("ItemId", "Date", Name = "UQ_ItemPrices_ItemDate", IsUnique = true)]
public partial class ItemPrice
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("GUID")]
    public Guid Guid { get; set; }

    [Column("ItemID")]
    public long ItemId { get; set; }

    public DateTime Date { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Column("CancellationPolicyID")]
    public long CancellationPolicyId { get; set; }

    [InverseProperty("ItemPrice")]
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    [ForeignKey("CancellationPolicyId")]
    [InverseProperty("ItemPrices")]
    public virtual CancellationPolicy CancellationPolicy { get; set; } = null!;

    [ForeignKey("Date")]
    [InverseProperty("ItemPrices")]
    public virtual DimDate DateNavigation { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("ItemPrices")]
    public virtual Item Item { get; set; } = null!;
}
