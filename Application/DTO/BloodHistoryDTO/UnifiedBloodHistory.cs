using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BloodHistoryDTO
{
    public class UnifiedBloodHistory
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // "Donation" or "Volunteer"
        public string FacilityName { get; set; } = string.Empty;
        public string? EventName { get; set; }
        public DateOnly? EventDate { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public DateOnly RegisterDate { get; set; }

        // Only for volunteer
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? IsExpired { get; set; }
    }
}

