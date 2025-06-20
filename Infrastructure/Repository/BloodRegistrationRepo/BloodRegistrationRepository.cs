using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.BloodRegistrationRepo
{
    public class BloodRegistrationRepository : GenericRepository<BloodRegistration>, IBloodRegistrationRepository
    {
        public BloodRegistrationRepository(BloodDonationSystemContext context) : base(context)
        {
        }

        public async Task<PaginatedResult<BloodRegistration>> GetPagedAsync(int pageNumber, int pageSize)
        {
             var bloodRegis = await _dbSet
                                    .OrderByDescending(e => e.CreateAt)
                                    .Skip(pageSize * (pageNumber - 1))
                                    .Take(pageSize)
                                    .ToListAsync();

            var pagedResult = new PaginatedResult<BloodRegistration>
            {
                Items = bloodRegis,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = await _dbSet.CountAsync()
            };
            return pagedResult;
        }
    }
}
