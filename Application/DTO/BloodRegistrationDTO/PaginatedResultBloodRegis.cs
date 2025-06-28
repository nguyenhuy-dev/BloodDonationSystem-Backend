using Infrastructure.Helper;

namespace Application.DTO.BloodRegistrationDTO
{
    public class PaginatedResultBloodRegis : PaginatedResult<BloodRegistrationResponse>
    {
        public DateOnly EventTime { get; set; }
    }
}
