namespace Application.DTO.EventsDTO
{
    public class UrgentEventResponse
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public double EstimatedVolume { get; set; }
        public string BloodTypeName { get; set; }
        public DateOnly EventTime { get; set; }
        public DateTime CreateAt { get; set; }
        public decimal Distance { get; set; }
    }
}
