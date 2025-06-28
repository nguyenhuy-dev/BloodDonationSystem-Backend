using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.BloodRegistrationRepo
{
    public interface IBloodRegistrationRepository : IGenericRepository<BloodRegistration>
    {
        Task<PaginatedResult<BloodRegistration>> GetPagedAsync(int eventId, int pageNumber, int pageSize);
    }
}
