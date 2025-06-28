using Application.DTO.BloodRegistration;
using Application.DTO.BloodRegistrationDTO;
using Domain.Entities;

namespace Application.Service.BloodRegistrationServ
{
    public interface IBloodRegistrationService
    {
        Task<BloodRegistration?> RegisterDonation(int id, BloodRegistrationRequest request);
        Task<BloodRegistration?> RejectBloodRegistration(int bloodRegisId);
        Task<BloodRegistration?> CancelOwnRegistration(int bloodRegisId);
        Task<PaginatedResultBloodRegis?> GetBloodRegistrationsByPaged(int eventId, int pageNumber, int pageSize);
    }
}
