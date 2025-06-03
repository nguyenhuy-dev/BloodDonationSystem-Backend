using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int MaxOfDonor { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime StartTime { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual ICollection<EventBloodType> EventBloodTypes { get; set; } = new List<EventBloodType>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
