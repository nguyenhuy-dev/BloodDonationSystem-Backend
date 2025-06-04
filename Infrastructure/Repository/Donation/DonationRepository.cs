using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Donation
{
    public class DonationRepository : IDonationRepository
    {
        private readonly BloodDonationSystemContext _context;

        public DonationRepository(BloodDonationSystemContext context)
        {
            _context = context;
        }

        public async Task<Registration> CreateRegistrationAsync(Registration registration)
        {
            _context.Registrations.Add(registration);
            await SaveAsync();
            return registration;
        }

        public Task<User> GetUserByIdAsync(Guid userId)
            => _context.Users.FindAsync(userId).AsTask();

        public Task<Event> GetEventByIdAsync(int eventId)
            => _context.Events.FindAsync(eventId).AsTask();

        public async Task<DonationHistory> CreateDonationHistoryAsync(DonationHistory history)
        {
            _context.DonationHistories.Add(history);
            await SaveAsync();
            return history;
        }

        public Task<DonationHistory> GetDonationHistoryByIdAsync(int id)
            => _context.DonationHistories
                .Include(d => d.Registration)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(d => d.Id == id);

        public Task AddProcessStepAsync(DonationProcessStep step)
        {
            _context.DonationProcessSteps.Add(step);
            return Task.CompletedTask;
        }

        public Task<BloodComponent> GetDefaultBloodComponentAsync()
            => _context.BloodComponents.FirstAsync(); // giả định default là cái đầu tiên

        public Task AddToInventoryAsync(Inventory inventory)
        {
            _context.Inventories.Add(inventory);
            return Task.CompletedTask;
        }

        public Task SaveAsync() => _context.SaveChangesAsync();
    }
}
