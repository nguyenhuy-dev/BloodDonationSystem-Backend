using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class BloodComponent
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
