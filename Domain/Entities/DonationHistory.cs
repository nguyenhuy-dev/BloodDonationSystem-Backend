using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class DonationHistory
{
    public int Id { get; set; }
    public DateTime CreateAt { get; set; }
    public float Volume { get; set; }
    public bool HealthStatus { get; set; }
    public bool BloodStatus { get; set; }
    public int RegistrationId { get; set; }

    public Registration Registration { get; set; }
    public Inventory Inventory { get; set; }
    public ICollection<DonationProcessStep> ProcessSteps { get; set; }
}
