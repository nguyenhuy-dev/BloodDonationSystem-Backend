using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.BloodInventoryRepo
{
    public class BloodInventoryRepository : GenericRepository<BloodInventory>, IBloodInventoryRepository
    {
        public BloodInventoryRepository(BloodDonationSystemContext context) : base(context)
        {

        }

        public async Task<int> GetBloodUnitsExpiredAsync()
        {
            var expiredBloodUnits = await _dbSet
                .Where(bu => bu.IsAvailable == true && bu.ExpiredDate <= DateTime.Now)
                .ToListAsync();

            foreach (var bu in expiredBloodUnits)
                bu.IsAvailable = false;

            return await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<BloodInventory>> GetBloodUnitsByPagedAsync(int pageNumber, int pageSize)
        {
            var bloodUnitsCount = await _dbSet
                                            .Where(bu => bu.IsAvailable == true)
                                            .ToListAsync();

            var bloodUnits = bloodUnitsCount
                                .OrderBy(bu => bu.CreateAt)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            var bloodUnitsPaged = new PaginatedResult<BloodInventory>
            {
                Items = bloodUnits,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = bloodUnitsCount.Count()
            };

            return bloodUnitsPaged;
        }

        public async Task<BloodInventory?> GetByBloodRegisIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(i => i.RegistrationId == id);
        }
    }
}
