using Application.DTO.VolunteerDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.VolunteerServ
{
    public interface IVolunteerService
    {
        Task<Volunteer?> RegisterVolunteerDonation(RegisterVolunteerDonation request);
        Task<Volunteer?> UpdateVolunteerDonation(int id, UpdateVolunteerDonation request);
        Task<PaginatedResult<VolunteersResponse>?> GetVolunteersByPaged(int pageNumber, int pageSize); 
    }
}
