using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;

namespace Infrastructure.Repository.BloodInventoryRepo
{
    public interface IBloodInventoryRepository : IGenericRepository<BloodInventory>
    {
        Task<BloodInventory?> GetByBloodRegisIdAsync(int id);
        Task<PaginatedResult<BloodInventory>> GetBloodUnitsByPagedAsync(int pageNumber, int pageSize);
    }
}
