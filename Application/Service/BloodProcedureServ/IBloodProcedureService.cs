using Application.DTO.BloodProcedureDTO;
using Domain.Entities;

namespace Application.Service.BloodProcedureServ
{
    public interface IBloodProcedureService
    {
        Task<BloodProcedure?> RecordBloodCollectionAsync(int id, BloodCollectionRequest request);
        Task<BloodProcedure?> UpdateBloodQualificationAsync(int regisId, RecordBloodQualification request);
        Task<PaginatedResultBloodProce?> GetBloodCollectionsByPaged(int eventId, int pageNumber, int pageSize);
    }
}
