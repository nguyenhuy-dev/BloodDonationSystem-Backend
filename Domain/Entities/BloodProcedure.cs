using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class BloodProcedure
    {
        public int Id { get; set; }
        public float Volume { get; set; }
        public bool? IsQualified { get; set; }
        public bool? HIV { get; set; }
        public bool? HCV { get; set; }
        public bool? Syphilis { get; set; }
        public float? Hematocrit { get; set; }
        public DateTime PerformedAt { get; set; }
        public string? Description { get; set; }

        public int? BloodTypeId { get; set; }
        public BloodComponent? BloodComponent { get; set; }
        public Guid PerformedBy { get; set; }


        [ForeignKey("BloodTypeId")]
        public virtual BloodType BloodType { get; set; }

        [ForeignKey("PerformedBy")]
        public virtual User PerformedByUser { get; set; }

        public virtual BloodRegistration BloodRegistration { get; set; }
    }
}
