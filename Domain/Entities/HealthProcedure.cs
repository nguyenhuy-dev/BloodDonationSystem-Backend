using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HealthProcedure
    {
        [Key]
        public int Id { get; set; }
        public int Pressure { get; set; }
        public float Temperature { get; set; }
        public float Hb { get; set; }
        public bool HBV { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public bool IsHealth { get; set; }
        public DateTime PerformedAt { get; set; }
        public string? Description { get; set; }

        public Guid PerformedBy { get; set; }

        [ForeignKey("PerformedBy")]
        public virtual User PerformedByUser { get; set; }

        public virtual BloodRegistration BloodRegistration { get; set; }
    }
}
