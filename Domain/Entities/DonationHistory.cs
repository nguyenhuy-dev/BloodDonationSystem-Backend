using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class DonationHistory
{
    public int Id { get; set; }

    public int RegistrationId { get; set; }

    public DateTime CreateAt { get; set; }

    public double? Volume { get; set; }

    public bool? HealthStatus { get; set; }

    public bool? BloodStatus { get; set; }

    public virtual ICollection<DonationProcessStep> DonationProcessSteps { get; set; } = new List<DonationProcessStep>();

    public virtual Inventory? Inventory { get; set; }

    public virtual Registration Registration { get; set; } = null!;
}
