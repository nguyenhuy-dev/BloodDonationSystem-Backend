using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.VolunteerRepo
{
    public class VolunteerRepository : GenericRepository<Volunteer>, IVolunteerRepository
    {
        public VolunteerRepository(BloodDonationSystemContext context) : base(context)
        {
        }

        public async Task<PaginatedResult<Volunteer>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var volunteers = await _dbSet
                .OrderByDescending(e => e.CreateAt)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            var pagedResult = new PaginatedResult<Volunteer>
            {
                Items = volunteers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = await _dbSet.CountAsync()
            };
            return pagedResult;
        }
    }
}
