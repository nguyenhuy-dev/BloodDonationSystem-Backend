using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BloodProcedure
    {
        public int Id { get; set; }
        public bool HIV { get; set; }
        public bool HBV { get; set; }
        public bool HCV { get; set; }
        public bool Syphilis { get; set; }
        public float Hb { get; set; }
        public float Hct { get; set; }
        public bool Status { get; set; }
        public DateTime PerformedAt { get; set; }

        public Guid PerformedBy { get; set; }
        public User Performer { get; set; }

        public int BloodTypeId { get; set; }
        public BloodType BloodType { get; set; }

        public int BloodComponentId { get; set; }
        public BloodComponent BloodComponent { get; set; }

        public DonationHistory DonationHistory { get; set; }
    }
}
