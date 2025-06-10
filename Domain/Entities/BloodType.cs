using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class BloodType
{
    [Key]
    public int Id { get; set; }

    [StringLength(10)]
    public string Type { get; set; } = null!;
    public ICollection<User> Users { get; set; }
    public ICollection<BloodCompatibility> Donors { get; set; }
    public ICollection<BloodCompatibility> Recipients { get; set; }
    public ICollection<BloodInventory> BloodInventories { get; set; }
    public ICollection<Event> Events { get; set; }
    public ICollection<BloodProcedure> BloodProcedures { get; set; }
}
