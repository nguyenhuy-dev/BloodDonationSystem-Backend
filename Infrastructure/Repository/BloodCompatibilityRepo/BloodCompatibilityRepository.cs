using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.BloodCompatibilityRepo
{
    public class BloodCompatibilityRepository(BloodDonationSystemContext _context) : IBloodCompatibilityRepository
    {
        public async Task<List<BloodCompatibility>> GetCompatibilityDonors(int receipientTypeId, BloodComponent component)
        {
            return await _context.BloodCompatibilities
                .Include(b => b.RecipientType)
                .Include(b => b.DonorType)
                .Where(b => b.RecipientTypeId == receipientTypeId && b.BloodComponent == component)
                .ToListAsync();
        }

        public async Task<List<BloodCompatibility>> GetCompatibilityRecipients(int donorTypeId, BloodComponent component)
        {
            return await _context.BloodCompatibilities
                .Include(b => b.RecipientType)
                .Include(b => b.DonorType)
                .Where(b => b.DonorTypeId == donorTypeId && b.BloodComponent == component)
                .ToListAsync();
        }
    }
}