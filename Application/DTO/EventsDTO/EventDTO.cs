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
        public int Id { get; set; }
        public string Title { get; set; }
        public int MaxOfDonor { get; set; }
        public double EstimatedVolume { get; set; }
        //public DateTime? UpdateAt { get; set; }
        public bool IsUrgent { get; set; }
        public DateOnly EventTime { get; set; }
        public string? BloodType { get; set; } = null;
        public string? BloodComponent { get; set; } = null;
        public int BloodRegisCount { get; set; }
    }
}
