using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BloodProcedure
    {
        public int Id { get; set; }
        public float Volume { get; set; }
        public bool IsQualified { get; set; }
        public DateTime PerformedAt { get; set; }
        public string? Description { get; set; }

        public int BloodTypeId { get; set; }
        public BloodComponent BloodComponent { get; set; }
        public Guid PerformedBy { get; set; }


        [ForeignKey("BloodTypeId")]
        public virtual BloodType BloodType { get; set; }

        [ForeignKey("PerformedBy")]
        public virtual User PerformedByUser { get; set; }

        public virtual BloodRegistration BloodRegistration { get; set; }
    }
}
