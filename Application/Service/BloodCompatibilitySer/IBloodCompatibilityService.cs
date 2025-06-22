using Domain.Enums;

namespace Application.Service.BloodCompatibilitySer
{
    public interface IBloodCompatibilityService
    {
        Task<string> GetCompatibilityDonors(int receiptTypeId, BloodComponent component);

        Task<string> GetCompatibilityRecipients(int donorTypeId, BloodComponent component);
    }
}