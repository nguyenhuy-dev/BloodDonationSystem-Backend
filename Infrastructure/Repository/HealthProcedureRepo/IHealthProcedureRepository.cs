using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.HealthProcedureRepo
{
    public interface IHealthProcedureRepository : IGenericRepository<HealthProcedure>
    {
        Task<PaginatedResult<HealthProcedure>> GetHealthProceduresByPaged(int id, int pageNumber, int pageSize);

        Task<List<HealthProcedure>> SearchHealthProceduresByNameOrPhoneAsync(int pageNumber, int pageSize, string keyword);

        Task<HealthProcedure?> GetIncludeByIdAsync(int id);
    }
}
