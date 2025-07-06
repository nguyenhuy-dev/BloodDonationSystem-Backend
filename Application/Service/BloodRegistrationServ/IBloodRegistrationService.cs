using Application.DTO;
using Application.DTO.BloodRegistration;
using Application.DTO.BloodRegistrationDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.BloodRegistrationServ
{
    public interface IBloodRegistrationService
    {
        Task<ApiResponse<BloodRegistration>?> RegisterDonation(int id, BloodRegistrationRequest request);
        Task<BloodRegistration?> RejectBloodRegistration(int bloodRegisId);
        Task<ApiResponse<BloodRegistration>?> CancelOwnRegistration(int bloodRegisId);
        Task<PaginatedResultBloodRegis?> GetBloodRegistrationsByPaged(int eventId, int pageNumber, int pageSize);

        Task<PaginatedResult<BloodRegistrationResponse>?> SearchBloodRegistrationsByPhoneOrName(int pageNumber, int pageSize, string keyword);

        Task<int> GetBloodRegistrationExpiredAsync();
    }
}
