using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.EventsDTO
{
    public class NormalEventDTO
    {
        public string Title { get; set; }
        public int MaxOfDonor { get; set; }
        public double EstimateVolume { get; set; }
        public DateTime EventTime { get; set; }
        public bool IsUrgent { get; set; } = false; // true: EmergencyEvent, false: NormalEvent
        public bool IsExprired { get; set; }

    }
}
