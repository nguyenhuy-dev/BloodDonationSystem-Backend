namespace Application.DTO.EventsDTO
{
    public class EventForDashAdminDTO
    {
        public int Id { get; set; }
        public DateOnly EventTime { get; set; }
        public string Address { get; set; } = string.Empty;
        public int SuccessfulBloodRegisCount { get; set; }
        public int BloodRegisCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsExpired { get; set; }
        public bool IsUrgent { get; set; }
        public string? BloodType { get; set; } = null;
        public string? BloodComponent { get; set; } = null;
        public int? BloodTypeId { get; set; }
    }
}
