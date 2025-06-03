using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BloodTypeCompatibility
    {
        public int Id { get; set; }
        public int DonorTypeId { get; set; }
        public int RecipientTypeId { get; set; }

        public BloodType DonorType { get; set; }
        public BloodType RecipientType { get; set; }
    }
}
