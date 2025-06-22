using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Repository.BloodCompatibilityRepo
{
    public interface IBloodCompatibilityRepository
    {
        Task<List<BloodCompatibility>> GetCompatibilityDonors(int receipientTypeId, BloodComponent component);

        Task<List<BloodCompatibility>> GetCompatibilityRecipients(int donorTypeId, BloodComponent component);
    }
}