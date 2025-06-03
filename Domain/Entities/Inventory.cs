using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Inventory
{
    public int Id { get; set; }
    public int BloodTypeId { get; set; }
    public int BloodComponentId { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime ExpiredDate { get; set; }
    public int DonationHistoryId { get; set; }

    public BloodType BloodType { get; set; }
    public BloodComponent BloodComponent { get; set; }
    public DonationHistory DonationHistory { get; set; }
}
