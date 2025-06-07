using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class BloodComponent
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Principle { get; set; } = null!;
    public ICollection<BloodProcedure> BloodProcedures { get; set; }
}
