using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.EventsDTO
{
    public class EventDTO
    {
        public string Title { get; set; }
        public int MaxOfDonor { get; set; }
        public double EstimatedVolume { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsUrgent { get; set; }
        public DateTime EventTime { get; set; }
        public bool IsExpired { get; set; }

        public int? BloodTypeId { get; set; }
        public BloodComponent? BloodComponent { get; set; }
    }
}
