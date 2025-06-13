using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //Tach pagination ra
        public async Task<PaginatedResult<Event>> GetAllEventAsync(int pageNumber, int pageSize)
        {
            var totalItems = await _context.Events.CountAsync();
            var items = await _context.Events
                .OrderByDescending(e => e.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Event>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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
