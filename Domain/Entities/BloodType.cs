using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class BloodType
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public ICollection<User> Users { get; set; }
    public ICollection<BloodTypeCompatibility> Donors { get; set; }
    public ICollection<BloodTypeCompatibility> Recipients { get; set; }
    public ICollection<Inventory> Inventories { get; set; }
}
