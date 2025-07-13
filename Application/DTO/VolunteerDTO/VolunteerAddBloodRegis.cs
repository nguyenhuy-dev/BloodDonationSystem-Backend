using Domain.Entities;

namespace Application.DTO.VolunteerDTO
{
    public class VolunteerAddBloodRegis
    {
        public Volunteer? Volunteer { get; set; }
        public BloodRegistration? BloodRegistration { get; set; }
    }
}
