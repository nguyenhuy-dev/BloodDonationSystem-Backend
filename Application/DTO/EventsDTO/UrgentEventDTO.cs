using Domain.Enums;

namespace Application.DTO.EventsDTO
{
    public class UrgentEventDTO
    {
        public string Title { get; set; }
        public int MaxOfDonor { get; set; }
        public double EstimatedVolume { get; set; }
        public DateOnly EventTime { get; set; }
        public string BloodType { get; set; }
        public BloodComponent BloodComponent { get; set; }
    }
}
