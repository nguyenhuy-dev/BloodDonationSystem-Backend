namespace Application.DTO.BloodRegistrationDTO
{
    public class BloodRegistrationResponse
    {
        public int Id { get; set; }
        public string MemberName { get; set; }
        public string? Phone { get; set; }
        public string? Type { get; set; }
        public DateOnly EventTime { get; set; }
    }
}
