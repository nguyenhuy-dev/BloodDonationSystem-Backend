using Application.DTO.BloodProcedureDTO;
using Domain.Entities;
using Application.DTO;
using Infrastructure.Helper;

namespace Application.Service.BloodProcedureServ
{
    public interface IBloodProcedureService
    {
        Task<ApiResponse<BloodProcedure>?> RecordBloodCollectionAsync(int id, BloodCollectionRequest request);
        Task<ApiResponse<BloodProcedure>?> UpdateBloodQualificationAsync(int regisId, RecordBloodQualification request);
        Task<PaginatedResultBloodProce?> GetBloodCollectionsByPaged(int eventId, int pageNumber, int pageSize);

        Task<PaginatedResult<BloodCollectionResponse>?> SearchBloodCollectionsByPhoneOrName(int pageNumber, int pageSize, string keyword);
    }
}
