using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.BloodProcedureRepo
{
    public class BloodProcedureRepository : GenericRepository<BloodProcedure>, IBloodProcedureRepository
    {
        public BloodProcedureRepository(BloodDonationSystemContext context) : base(context)
        {
        }

        // Lấy list những đơn đăng ký mà chưa kiểm tra chất lượng máu
        public async Task<PaginatedResult<BloodProcedure>> GetBloodCollectionsByPagedAsync(int eventId, int pageNumber, int pageSize)
        {
            var bloodCollectionsByEvent = await _dbSet
                .Include(bc => bc.BloodRegistration)
                    .ThenInclude(br => br.Event)
                .Include(bc => bc.BloodRegistration)
                    .ThenInclude(br => br.Member)
                .Where(bc => bc.BloodRegistration.EventId == eventId &&
                                bc.IsQualified == null)
                .ToListAsync();

            var bloodCollections = bloodCollectionsByEvent
                .OrderBy(bc => bc.PerformedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PaginatedResult<BloodProcedure>
            {
                Items = bloodCollections,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = bloodCollectionsByEvent.Count()
            };
            return pagedResult;
        }

        public async Task<List<BloodProcedure>> SearchBloodCollectionsByPhoneOrNameAsync(int pageNumber, int pageSize, string keyword, int? eventId = null)
        {
            IQueryable<BloodProcedure> query = _context.BloodProcedures
                .Include(bc => bc.BloodRegistration)
                    .ThenInclude(br => br.Member)
                        .ThenInclude(mem => mem.BloodType)
                .Include(bc => bc.BloodRegistration)
                    .ThenInclude(br => br.Event)
                .Where(bp => bp.IsQualified == null);

            if (IsPhone(keyword))
            {
                query = query.Where(bc => bc.BloodRegistration.Member.Phone.Contains(keyword));
            }
            else
            {
                query = query.Where(bc => bc.BloodRegistration.Member.LastName.Contains(keyword) || 
                                        bc.BloodRegistration.Member.FirstName.Contains(keyword));
            }

            if(eventId != null)
            {
                query = query.Where(bc => bc.BloodRegistration.EventId == eventId);
            }

            return await query
                .OrderBy(bc => bc.PerformedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private bool IsPhone(string keyword)
        {
            return keyword.All(char.IsDigit);
        }
    }
}