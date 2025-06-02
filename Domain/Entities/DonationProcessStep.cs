using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class DonationProcessStep
{
    public int Id { get; set; }

    public int DonationHistoryId { get; set; }

    public string StepName { get; set; } = null!;

    public Guid PerformedBy { get; set; }

    public DateTime PerformedAt { get; set; }

    public string? Description { get; set; }

    public virtual DonationHistory DonationHistory { get; set; } = null!;

    public virtual User PerformedByNavigation { get; set; } = null!;
}
