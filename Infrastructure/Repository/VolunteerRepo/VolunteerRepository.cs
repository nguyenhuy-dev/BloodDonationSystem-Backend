using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;
using Infrastructure.Repository.Facilities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.VolunteerRepo
{
    public class VolunteerRepository : GenericRepository<Volunteer>, IVolunteerRepository
    {
        private readonly IFacilityRepository _repoFacility;

        public VolunteerRepository(BloodDonationSystemContext context, IFacilityRepository repoFacility) : base(context)
        {
            _repoFacility = repoFacility;
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
            // Hard code cho 1 cơ sở
            var facility = await _repoFacility.GetByIdAsync(1);
            if (facility == null)
                throw new ArgumentNullException("Null");

            decimal range = 0.1M;
            var volunteersRaw = await _dbSet
                .Include(v => v.Member)
                .Where(v => v.Member.Latitude >= facility.Latitude - range &&
                            v.Member.Latitude <= facility.Latitude + range &&
                            v.Member.Longitude >= facility.Longitude - range &&
                            v.Member.Longitude <= facility.Longitude + range)
                .ToListAsync();

            var volunteers = volunteersRaw
                .Select(vr => new
                {
                    Volunteer = vr,
                    Distance = GeographyHelper.CalculateDistanceKm(facility.Latitude, facility.Longitude, vr.Member.Latitude, vr.Member.Longitude)
                })
                .Where(vr => vr.Distance <= 5 && vr.Volunteer.IsExpired == false);  //hard code 5km

            var volunteersPaged = volunteers
                .OrderBy(vr => vr.Distance)
                    .ThenBy(vr => vr.Volunteer.CreateAt)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToList();
                

            var pagedResult = new PaginatedResult<Volunteer>
            {
                Items = volunteersPaged.Select(v => v.Volunteer).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = volunteers.Count()
            };
            return pagedResult;
        }

        public async Task<IEnumerable<Volunteer>?> GetVolunteerByMemberIdAsync(Guid memberId)
        {
            return await _dbSet
                        .Where(v => v.MemberId == memberId)
                        .ToListAsync();
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