using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Donation
{
    public interface IDonationRepository
    {
        Task<Registration> CreateRegistrationAsync(Registration registration);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<Event> GetEventByIdAsync(int eventId);

        Task<DonationHistory> CreateDonationHistoryAsync(DonationHistory history);
        Task<DonationHistory> GetDonationHistoryByIdAsync(int id);

        Task AddProcessStepAsync(DonationProcessStep step);

        Task<BloodComponent> GetDefaultBloodComponentAsync();
        Task AddToInventoryAsync(Inventory inventory);

        Task SaveAsync();
    }
}
