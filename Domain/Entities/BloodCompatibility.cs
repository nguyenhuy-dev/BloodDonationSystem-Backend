using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BloodCompatibility
    {
        public int Id { get; set; }
        public BloodComponent BloodComponent { get; set; }
        public int BloodTypeId { get; set; }
        public int DonorTypeId { get; set; }
        public int RecipientTypeId { get; set; }
        [ForeignKey("BloodTypeId")]
        public BloodType BloodType { get; set; }
        [ForeignKey("DonorTypeId")]
        public BloodType DonorType { get; set; }
        [ForeignKey("RecipientTypeId")]
        public BloodType RecipientType { get; set; }
    }
}
