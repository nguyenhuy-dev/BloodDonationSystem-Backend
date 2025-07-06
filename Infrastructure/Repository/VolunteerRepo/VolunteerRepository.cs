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

        public async Task<int> EndVolunteerDateExpired()
        {
            var today = DateTime.Now;
            var expiredVolunteers = _context.Volunteers
                .Where(v => v.EndVolunteerDate < today)
                .ToListAsync();

            foreach(var expiredVolunteer in expiredVolunteers.Result)
            {
                expiredVolunteer.IsExpired = true;
            }

            return await _context.SaveChangesAsync();
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

        public async Task<bool> UpdateAvailableDateAsync(int id, DateTime startDate, DateTime endDate)
        {
            var volunteer = await GetByIdAsync(id);
            if(volunteer == null)
            {
                return false;
            }
            volunteer.StartVolunteerDate = startDate;
            volunteer.EndVolunteerDate = endDate;
            volunteer.UpdateAt = DateTime.Now;

            _context.Update(volunteer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
