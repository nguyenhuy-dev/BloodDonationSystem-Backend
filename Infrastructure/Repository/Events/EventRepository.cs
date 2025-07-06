using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Events
{
    public class EventRepository(BloodDonationSystemContext _context) : IEventRepository
    {
        public async Task<Event?> AddEventAsync(Event newEvent)
        {
            _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();
            return newEvent; // Return the newly added event
        }

        public async Task<int> CountAllEventAsync()
        {
            var count = await _context.Events.CountAsync();
            return count; // Return the total count of events
        }

        public async Task<int> CountAllActiveEventAsync()
        {
            var count = await _context.Events.Where(e => e.IsExpired == false).CountAsync();
            return count; // Return the count of all active events
        }

        //Tach pagination ra
        public async Task<List<Event>> GetAllUrgentEventAsync(int pageNumber, int pageSize)
        {
            return await _context.Events
                .Where(e => e.IsUrgent)
                .Include(e => e.BloodType) // Include related BloodType entity if needed
                .OrderByDescending(e => e.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Event>> GetAllNormalEventAsync(int pageNumber, int pageSize)
        {
            return await _context.Events
                .Where(e => !e.IsUrgent)
                .Include(e => e.BloodType) // Include related BloodType entity if needed
                .OrderByDescending(e => e.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Event>> GetAllEventAsync(int pageNumber, int pageSize)
        {
            return await _context.Events
                .Include(e => e.BloodType) // Include related BloodType entity if needed
                .OrderByDescending(e => e.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Event>> GetAllActiveEventAsync(int pageNumber, int pageSize)
        {
            return await _context.Events
                .Where(e => e.IsExpired == false)
                .Include(e => e.BloodType) // Include related BloodType entity if needed
                .OrderByDescending(e => e.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
            var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            return eventItem; // Return the event if found, otherwise null
        }

        public async Task<Event> UpdateEventAsync(Event updateEvent)
        {
            _context.Events.Update(updateEvent);
            await _context.SaveChangesAsync();
            return updateEvent; // Return the updated event
        }

        public async Task<int> EventExpiredAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var expiredEvents = _context.Events
                .Where(e => e.EventTime < today && !e.IsExpired)
                .ToListAsync();

            foreach (var expiredEvent in expiredEvents.Result)
            {
                expiredEvent.IsExpired = true;
            }

            return await _context.SaveChangesAsync();
        }

        //Khong nhan event het han, khong nhan event khong don
        public async Task<List<Event>> GetPassedHealthProcedureAsync(int pageNumber, int pageSize)
        {
            var events = await _context.Events
                .Where(e => e.IsExpired == false)
                .Include(e => e.BloodRegistrations.Where(br => br.HealthId != null && br.IsApproved == true && br.BloodProcedureId == null))
                .OrderByDescending(e => e.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return events;
        }

        //Nhan event het han + khong hien event khong don
        public async Task<List<Event>> GetEventListDoBloodProcedure(int pageNumber, int pageSize)
        {
            var events = await _context.Events
                .Include(e => e.BloodRegistrations)
                .ThenInclude(br => br.BloodProcedure)
                .ToListAsync();

            var filtered = events.Select(e => new Event
            {
            Id = e.Id,
            Title = e.Title,
            EventTime = e.EventTime,
            Facility = e.Facility,
            BloodRegistrations = e.BloodRegistrations
                .Where(br =>
                    br.HealthId != null &&
                    br.IsApproved == true &&
                    (br.BloodProcedureId != null && br.BloodProcedure.IsQualified == null)
                ).ToList()
            })
                .Where(e => e.BloodRegistrations.Any())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return filtered;
        }

        public async Task<int> CountEventPassedHealthProcedureAsync()
        {
            return await _context.Events
                .Where(e => e.IsExpired == false &&
                            e.BloodRegistrations.Any(br =>
                                br.IsApproved == true &&
                                br.HealthId != null &&
                                br.BloodProcedureId == null))
                .CountAsync();
        }

        public async Task<int> CountEventListDoBloodProcedure()
        {
            return await _context.Events
                .Where(e => e.BloodRegistrations.Any(br =>
                        br.HealthId != null &&
                        br.IsApproved == true &&
                        br.BloodProcedureId != null &&
                        br.BloodProcedure.IsQualified == null))
                .CountAsync();

            //var totalCount = events
            //    .SelectMany(e => e.BloodRegistrations)
            //    .Count(br =>
            //        br.HealthId != null &&
            //        br.IsApproved == true &&
            //        br.BloodProcedureId != null &&
            //        br.BloodProcedure?.IsQualified == null
            //    );

            //return totalCount;
        }

        public async Task<List<Event>> SearchEventByDayAsync(DateOnly? startDay, DateOnly? endDay)
        {
            return await _context.Events
                .Include(e => e.BloodType)
                .Include(e => e.BloodRegistrations)
                .Where(e => e.EventTime >= startDay && e.EventTime <= endDay)
                .ToListAsync();
        }

        public async Task<int> CountEventFromDayToDay(DateOnly? startDay, DateOnly? endDay)
        {
            return await _context.Events
                .Include(e => e.BloodType)
                .Include(e => e.BloodRegistrations)
                .Where(e => e.EventTime >= startDay && e.EventTime <= endDay)
                .CountAsync();
        }
    }
}