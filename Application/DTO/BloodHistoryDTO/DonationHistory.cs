using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BloodHistoryDTO
{
    public class DonationHistory
    {
        public int RegistrationId { get; set; }
        public DateOnly DonateDate { get; set; }
        public string FacilityName { get; set; }
        public string FacilityAddress { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public bool? Status { get; set; }
        public float? Volume { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
