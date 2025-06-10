using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; }
    [StringLength(100)]
    public string LastName { get; set; }
    public bool? Gender { get; set; }
    public DateOnly? Dob { get; set; }

    [StringLength(10)]
    public string? Phone { get; set; }
    public string? HashPass { get; set; }

    public string? Gmail { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }

    public DateTime? LastDonation { get; set; }
    public bool IsActived { get; set; }

    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }

    public Guid? UpdateBy { get; set; }
    public int RoleId { get; set; }
    public int? BloodTypeId { get; set; }

    [ForeignKey("UpdateBy")]
    public virtual User UpdatedByUser { get; set; }

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; }

    [ForeignKey("BloodTypeId")]
    public virtual BloodType BloodType { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<Event> CreatedEvents { get; set; }
    public virtual ICollection<Event> UpdatedEvents { get; set; }
    public virtual ICollection<BloodRegistration> MemberRegistrations { get; set; }
    public virtual ICollection<BloodRegistration> StaffRegistrations { get; set; }
    public virtual ICollection<Volunteer> Volunteers { get; set; }
    public virtual ICollection<HealthProcedure> HealthProcedures { get; set; }
    public virtual ICollection<BloodProcedure> BloodProcedures { get; set; }
    public virtual ICollection<BloodInventory> BloodInventories { get; set; }
    public virtual ICollection<Blog> Blogs { get; set; }
    public virtual ICollection<Comment> MemberComments { get; set; }
    public virtual ICollection<Comment> StaffComments { get; set; }
}
