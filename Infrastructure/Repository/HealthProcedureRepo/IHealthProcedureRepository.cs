using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.HealthProcedureRepo
{
    public interface IHealthProcedureRepository : IGenericRepository<HealthProcedure>
    {
        Task<PaginatedResult<HealthProcedure>> GetHealthProceduresByPaged(int id, int pageNumber, int pageSize);
        Task<HealthProcedure?> GetIncludeByIdAsync(int id);
    }
}
