using Application.DTO.HealthProcedureDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.HealthProcedureServ
{
    public interface IHealthProcedureService
    {
        Task<HealthProcedure?> RecordHealthProcedureAsync(int id, HealthProcedureRequest request);
        Task<object?> GetHealthProceduresByPagedAsync(int id, int pageNumber, int pageSize);
        Task<HealthProcedure?> CancelHealthProcessAsync(int bloodRegisId);
    }
}
