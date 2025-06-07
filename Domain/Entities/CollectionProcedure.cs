using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CollectionProcedure
    {
        public int Id { get; set; }
        public float Volume { get; set; }
        public DateTime PerformedAt { get; set; }
        public Guid PerformedBy { get; set; }
        public User PerformedByUser { get; set; }

        public DonationHistory DonationHistory { get; set; }
    }
}
