using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class BloodType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<EventBloodType> EventBloodTypes { get; set; } = new List<EventBloodType>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<BloodType> DonorTypes { get; set; } = new List<BloodType>();

    public virtual ICollection<BloodType> RecipientTypes { get; set; } = new List<BloodType>();
}
