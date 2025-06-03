using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Registration
{
    public int Id { get; set; }
    public DateTime CreateAt { get; set; }
    public bool IsVolunteer { get; set; }
    public Guid UserId { get; set; }
    public int EventId { get; set; }

    public User User { get; set; }
    public Event Event { get; set; }
    public DonationHistory DonationHistory { get; set; }
}