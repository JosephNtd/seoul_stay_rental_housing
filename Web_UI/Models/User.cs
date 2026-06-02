using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_UI.Models;

[Index("Email", Name = "UQ_Users_Email", IsUnique = true)]
[Index("Guid", Name = "UQ_Users_GUID", IsUnique = true)]
[Index("Username", Name = "UQ_Users_Username", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("GUID")]
    public Guid Guid { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    public byte Gender { get; set; }

    public DateTime? BirthDate { get; set; }

    [StringLength(50)]
    public string? Country { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? ProfilePicture { get; set; }

    public bool IsAdmin { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AddonService> AddonServices { get; set; } = new List<AddonService>();

    [InverseProperty("ChangedByUser")]
    public virtual ICollection<BookingStatusHistory> BookingStatusHistories { get; set; } = new List<BookingStatusHistory>();

    [InverseProperty("User")]
    public virtual Guest? Guest { get; set; }

    [InverseProperty("User")]
    public virtual Host? Host { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<ItemScore> ItemScores { get; set; } = new List<ItemScore>();

    [InverseProperty("Reviewee")]
    public virtual ICollection<Review> ReviewReviewees { get; set; } = new List<Review>();

    [InverseProperty("Reviewer")]
    public virtual ICollection<Review> ReviewReviewers { get; set; } = new List<Review>();

    [InverseProperty("User")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
