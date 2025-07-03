using Application.DTO;
using Application.DTO.HealthProcedureDTO;
using Domain.Entities;

namespace Application.Service.HealthProcedureServ
{
    public interface IHealthProcedureService
    {
        Task<HealthProcedure?> RecordHealthProcedureAsync(int id, HealthProcedureRequest request);
        Task<object?> GetHealthProceduresByPagedAsync(int id, int pageNumber, int pageSize);
        Task<ApiResponse<HealthProcedure>?> CancelHealthProcessAsync(int bloodRegisId);
    }
}
