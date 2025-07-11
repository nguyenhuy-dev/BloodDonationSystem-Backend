using Application.DTO.EventsDTO;

namespace Application.DTO.VolunteerDTO
{
    public class UrgentEventVolunteer : UrgentEventDTO
    {
        public int[] VolunteerIds { get; set; }
    }
}
