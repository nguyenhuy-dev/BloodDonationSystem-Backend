using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.BloodRegistrationRepo
{
    public interface IBloodRegistrationRepository : IGenericRepository<BloodRegistration>
    {
        Task<PaginatedResult<BloodRegistration>> GetPagedAsync(int eventId, int pageNumber, int pageSize);
        Task<IEnumerable<BloodRegistration>> GetByEventAsync(int eventId);

        Task<List<BloodRegistration>> GetBloodRegistrationHistoryAsync(Guid userId);
        Task<List<BloodRegistration>> GetVolunteerRegistrationHistoryAsync(Guid userId);

        Task<List<BloodRegistration>> GetDonationHistoryAsync(Guid userId);

        Task<List<BloodRegistration>> SearchBloodRegistration(int pageNumber, int pageSize, string keyword, int? eventId = null);

        Task<int> BloodRegistrationExpiredAsync();
    }
}
