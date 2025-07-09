using Application.DTO;
using Application.DTO.HealthProcedureDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.HealthProcedureServ
{
    public interface IHealthProcedureService
    {
        Task<HealthProcedure?> RecordHealthProcedureAsync(int id, HealthProcedureRequest request);
        Task<object?> GetHealthProceduresByPagedAsync(int id, int pageNumber, int pageSize);
        Task<PaginatedResultWithEventTime<SearchHealthProcedureDTO>?> SearchHealthProceduresByPhoneOrNameAsync(int pageNumber, int pageSize, string keyword, int? eventId = null);
        Task<ApiResponse<HealthProcedure>?> CancelHealthProcessAsync(int bloodRegisId);
    }
}
