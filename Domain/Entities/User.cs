using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool Gender { get; set; }
    public DateOnly Dob { get; set; }
    public string Phone { get; set; } = null!;
    public string? Gmail { get; set; }
    public string HashPass { get; set; } = null!;
    public DateTime? LastDonation { get; set; }
    public string? Address { get; set; }
    //public Microsoft.SqlServer.Types.SqlGeography? Longtitude { get; set; }
    //public Microsoft.SqlServer.Types.SqlGeography? Latitude { get; set; }
    public int RoleId { get; set; }
    public int BloodTypeId { get; set; }

    public Role Role { get; set; }
    public BloodType BloodType { get; set; }
    public ICollection<Event> EventsCreated { get; set; }
    public ICollection<Registration> Registrations { get; set; }
    public ICollection<DonationProcessStep> ProcessSteps { get; set; }
    public ICollection<Blog> Blogs { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
