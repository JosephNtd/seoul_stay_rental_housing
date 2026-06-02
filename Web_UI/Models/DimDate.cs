using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_UI.Models;

[Index("Date", Name = "UQ_DimDates_Date", IsUnique = true)]
public partial class DimDate
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    public DateTime Date { get; set; }

    public int Year { get; set; }

    public int Quarter { get; set; }

    public int Month { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string MonthName { get; set; } = null!;

    public int DayOfMonth { get; set; }

    public int DayOfWeek { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string DayName { get; set; } = null!;

    public bool IsHoliday { get; set; }

    [InverseProperty("CheckInDateNavigation")]
    public virtual ICollection<Booking> BookingCheckInDateNavigations { get; set; } = new List<Booking>();

    [InverseProperty("CheckOutDateNavigation")]
    public virtual ICollection<Booking> BookingCheckOutDateNavigations { get; set; } = new List<Booking>();

    [InverseProperty("DateNavigation")]
    public virtual ICollection<ItemAvailability> ItemAvailabilities { get; set; } = new List<ItemAvailability>();

    [InverseProperty("DateNavigation")]
    public virtual ICollection<ItemPrice> ItemPrices { get; set; } = new List<ItemPrice>();

    [InverseProperty("TransactionDateNavigation")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
