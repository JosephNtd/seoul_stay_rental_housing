using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_UI.Models;

[Index("Guid", Name = "UQ_Transactions_GUID", IsUnique = true)]
public partial class Transaction
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("GUID")]
    public Guid Guid { get; set; }

    [Column("UserID")]
    public long UserId { get; set; }

    [Column("TransactionTypeID")]
    public long TransactionTypeId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    [Column("GatewayReturnID")]
    [StringLength(100)]
    public string GatewayReturnId { get; set; } = null!;

    [InverseProperty("Transaction")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("TransactionDate")]
    [InverseProperty("Transactions")]
    public virtual DimDate TransactionDateNavigation { get; set; } = null!;

    [ForeignKey("TransactionTypeId")]
    [InverseProperty("Transactions")]
    public virtual TransactionType TransactionType { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Transactions")]
    public virtual User User { get; set; } = null!;
}
