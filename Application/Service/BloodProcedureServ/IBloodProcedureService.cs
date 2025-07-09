using Application.DTO.BloodProcedureDTO;
using Domain.Entities;
using Application.DTO;
using Infrastructure.Helper;

namespace Application.Service.BloodProcedureServ
{
    public interface IBloodProcedureService
    {
        Task<ApiResponse<BloodProcedure>?> RecordBloodCollectionAsync(int id, BloodCollectionRequest request);
        Task<ApiResponse<RecordBloodQualification>?> UpdateBloodQualificationAsync(int regisId, RecordBloodQualification request);
        Task<PaginatedResultBloodProce?> GetBloodCollectionsByPaged(int eventId, int pageNumber, int pageSize);

        Task<PaginatedResultWithEventTime<SearchBloodProcedureDTO>?> SearchBloodCollectionsByPhoneOrName(int pageNumber, int pageSize, string keyword, int? eventId = null);
    }
}
