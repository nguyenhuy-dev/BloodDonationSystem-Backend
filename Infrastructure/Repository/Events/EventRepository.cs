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

        public async Task<int> CountAllAsync()
        {
            var count = await _context.Events.Where(e => e.IsExpired == false).CountAsync();
            return count; // Return the total count of events
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
    }
}