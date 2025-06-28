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

        public async Task<PaginatedResult<BloodRegistration>> GetPagedAsync(int eventId, int pageNumber, int pageSize)
        {
             var bloodRegistrations = await _dbSet
                                    .Include(br => br.Event)
                                    .Where(br => br.EventId == eventId)
                                    .OrderByDescending(e => e.CreateAt)
                                    .Skip(pageSize * (pageNumber - 1))
                                    .Take(pageSize)
                                    .ToListAsync();

            var pagedResult = new PaginatedResult<BloodRegistration>
            {
                Items = bloodRegistrations,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = await _dbSet.CountAsync(br => br.EventId == eventId)
            };
            return pagedResult;
        }
    }
}
