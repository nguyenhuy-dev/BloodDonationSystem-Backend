using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Inventory
{
    public int Id { get; set; }

    public DateTime CreateAt { get; set; }

    public int BloodTypeId { get; set; }

    public int BloodComponentId { get; set; }

    public int DonationHistoryId { get; set; }

    public DateTime ExpiredDate { get; set; }

    public virtual BloodComponent BloodComponent { get; set; } = null!;

    public virtual BloodType BloodType { get; set; } = null!;

    public virtual DonationHistory DonationHistory { get; set; } = null!;
}
