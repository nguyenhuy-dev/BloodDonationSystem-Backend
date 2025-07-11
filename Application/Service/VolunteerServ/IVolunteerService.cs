using Application.DTO;
using Application.DTO.VolunteerDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.VolunteerServ
{
    public interface IVolunteerService
    {
        Task<ApiResponse<Volunteer>?> RegisterVolunteerDonationAsync(RegisterVolunteerDonation request);
        Task<Volunteer?> UpdateVolunteerDonationAsync(int id, UpdateVolunteerDonation request);
        Task<PaginatedResult<VolunteersResponse>?> GetVolunteersByPagedAsync(int facilityId, int pageNumber, int pageSize);
        //Task<ApiResponse<Volunteer>> AddDonationRegistrationWithVolunteerAsync(int eventId, int id);
        Task<ApiResponseFindDonors> AddDonationRegistrationWithVolunteersAsync(UrgentEventVolunteer urgentEventVolunteer);

        Task<int> VolunteerEndDateExpiredAsync();
    }
}
