using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class DonationHistory
{
    public int Id { get; set; }
    public DateTime CreateAt { get; set; }
    public int HealthId { get; set; }
    public int CollectId { get; set; }
    public int QualifiedId { get; set; }
    public int RegistrationId { get; set; }

    public HealthProcedure HealthProcedure { get; set; }
    public CollectionProcedure CollectionProcedure { get; set; }
    public BloodProcedure BloodProcedure { get; set; }
    public Registration Registration { get; set; }
    public Inventory Inventory { get; set; }
}
