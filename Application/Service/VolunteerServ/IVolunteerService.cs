using Application.DTO;
using Application.DTO.VolunteerDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.VolunteerServ
{
    public interface IVolunteerService
    {
        Task<ApiResponse<Volunteer>?> RegisterVolunteerDonation(RegisterVolunteerDonation request);
        Task<Volunteer?> UpdateVolunteerDonation(int id, UpdateVolunteerDonation request);
        Task<ApiResponse<Volunteer>?> AddDonationRegistrationWithVolunteer(int eventId, int id);
        Task<PaginatedResult<VolunteersResponse>?> GetVolunteersByPaged(int pageNumber, int pageSize);

        Task<int> VolunteerEndDateExpiredAsync();
    }
}
