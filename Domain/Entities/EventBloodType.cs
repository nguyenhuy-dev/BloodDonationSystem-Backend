using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventBloodType
{
    public int Id { get; set; }

    public int BloodTypeId { get; set; }

    public int EventId { get; set; }

    public virtual BloodType BloodType { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
