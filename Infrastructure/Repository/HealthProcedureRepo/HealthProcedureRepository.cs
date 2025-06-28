using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.HealthProcedureRepo
{
    public class HealthProcedureRepository : GenericRepository<HealthProcedure>, IHealthProcedureRepository
    {
        public HealthProcedureRepository(BloodDonationSystemContext context) : base(context)
        {
        }

        public async Task<PaginatedResult<HealthProcedure>> GetHealthProceduresByPaged(int id, int pageNumber, int pageSize)
        {
            var healthProcedures = await _dbSet
                .Include(hp => hp.BloodRegistration)
                    .ThenInclude(br => br.Member)
                        .ThenInclude(mem => mem.BloodType)
                .Include(hp => hp.BloodRegistration)
                    .ThenInclude(br => br.Event)
                .Where(hp => hp.BloodRegistration.EventId == id)
                .OrderBy(hp => hp.PerformedAt)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            var pagedHealthProcedure = new PaginatedResult<HealthProcedure>
            {
                Items = healthProcedures,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = await _dbSet.CountAsync(hp => hp.BloodRegistration.EventId == id)
            };
            return pagedHealthProcedure;
        }
    }
}
