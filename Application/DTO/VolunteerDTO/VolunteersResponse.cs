namespace Application.DTO.VolunteerDTO
{
    public class VolunteersResponse
    {
        public int Id { get; set; }
        public string BloodTypeName { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Gmail { get; set; }
    }
}
