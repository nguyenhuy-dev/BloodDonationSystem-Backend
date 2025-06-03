using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class DonationProcessStep
{
    public int Id { get; set; }
    public string StepName { get; set; } = null!;
    public Guid PerformedBy { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? Description { get; set; }
    public int DonationHistoryId { get; set; }

    public User PerformedUser { get; set; }
    public DonationHistory DonationHistory { get; set; }
}