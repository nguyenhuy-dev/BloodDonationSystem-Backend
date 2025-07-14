namespace Application.DTO.BloodRegistrationDTO
{
    public class BloodRegistrationResponse
    {
        public int Id { get; set; }
        public string MemberName { get; set; }
        public string? Phone { get; set; }
        public DateOnly? Dob {  get; set; }
        public string? BloodType { get; set; }
        public bool? IsApproved { get; set; }
    }
}
