using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int MaxOfDonor { get; set; }
    public Guid CreateBy { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime StartTime { get; set; }
    public bool EventType { get; set; }
    public int FacilityId { get; set; }

    public User Creator { get; set; }
    public Facility Facility { get; set; }
    public ICollection<Registration> Registrations { get; set; }
}

