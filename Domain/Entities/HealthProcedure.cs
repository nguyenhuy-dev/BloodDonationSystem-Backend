using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HealthProcedure
    {
        public int Id { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Pressure { get; set; }
        public float Temperature { get; set; }
        public bool Status { get; set; }
        public DateTime PerformedAt { get; set; }
        public Guid PerformedBy { get; set; }
        public User Performer { get; set; }

        public DonationHistory DonationHistory;
    }
}
