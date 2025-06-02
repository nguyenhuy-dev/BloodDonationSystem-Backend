using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public bool Gender { get; set; }

    public DateOnly Dob { get; set; }

    public string Phone { get; set; } = null!;

    public string HashPass { get; set; } = null!;

    public DateOnly? LastDonation { get; set; }

    public string? Address { get; set; }

    public double? Area { get; set; }

    public string Role { get; set; } = null!;

    public int BloodTypeId { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual BloodType BloodType { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<DonationProcessStep> DonationProcessSteps { get; set; } = new List<DonationProcessStep>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
