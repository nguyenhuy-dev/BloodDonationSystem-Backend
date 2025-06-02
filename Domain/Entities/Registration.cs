using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Registration
{
    public int Id { get; set; }

    public DateTime CreateAt { get; set; }

    public Guid UserId { get; set; }

    public int EventId { get; set; }

    public virtual DonationHistory? DonationHistory { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
