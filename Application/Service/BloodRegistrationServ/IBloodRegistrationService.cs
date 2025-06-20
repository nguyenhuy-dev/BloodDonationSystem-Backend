using Application.DTO.BloodRegistration;
using Application.DTO.BloodRegistrationDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.BloodRegistrationServ
{
    public interface IBloodRegistrationService
    {
        Task<BloodRegistration?> RegisterDonation(int id, BloodRegistrationRequest request);
        Task<BloodRegistration?> RejectBloodRegistration(int bloodRegisId);
        Task<BloodRegistration?> CancelOwnRegistration(int bloodRegisId);
        Task<PaginatedResult<BloodRegistrationResponse>> GetBloodRegistrationsByPaged(int pageNumber, int pageSize);
    }
}
