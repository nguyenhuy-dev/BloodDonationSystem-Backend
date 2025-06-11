using Domain.Entities;
using Infrastructure.Data;
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
    }
}
