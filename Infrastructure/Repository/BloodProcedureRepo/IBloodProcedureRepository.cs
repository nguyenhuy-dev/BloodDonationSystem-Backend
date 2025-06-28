using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.BloodProcedureRepo
{
    public interface IBloodProcedureRepository : IGenericRepository<BloodProcedure>
    {
        Task<PaginatedResult<BloodProcedure>> GetBloodCollectionsByPagedAsync(int eventId, int pageNumber, int pageSize);
    }
}
