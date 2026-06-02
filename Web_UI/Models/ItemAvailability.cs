using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_UI.Models;

[Table("ItemAvailability")]
[Index("ItemId", "Date", Name = "UQ_ItemAvailability_ItemDate", IsUnique = true)]
public partial class ItemAvailability
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("ItemID")]
    public long ItemId { get; set; }

    public DateTime Date { get; set; }

    public bool IsAvailable { get; set; }

    [ForeignKey("Date")]
    [InverseProperty("ItemAvailabilities")]
    public virtual DimDate DateNavigation { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("ItemAvailabilities")]
    public virtual Item Item { get; set; } = null!;
}
