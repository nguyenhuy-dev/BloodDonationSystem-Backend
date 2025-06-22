using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repository.BloodCompatibilityRepo;

namespace Application.Service.BloodCompatibilitySer
{
    public class BloodCompatibilityService(IBloodCompatibilityRepository _bloodComRepo) : IBloodCompatibilityService
    {
        public async Task<string> GetCompatibilityDonors(int receiptTypeId, BloodComponent component)
        {
            var compatibility = await _bloodComRepo.GetCompatibilityDonors(receiptTypeId, component);
            if(compatibility == null)
            {
                return "No compatible donors found.";
            }
            return string.Join(", ", compatibility.Select(c => c.DonorType.Type));
        }

        public async Task<string> GetCompatibilityRecipients(int donorTypeId, BloodComponent component)
        {
            var compatibility = await _bloodComRepo.GetCompatibilityRecipients(donorTypeId, component);
            if(compatibility == null)
            {
                return "No compatible donors found";
            }
            return string.Join(", ", compatibility.Select(c => c.RecipientType.Type));
        }
    }
}