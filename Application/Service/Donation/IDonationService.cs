using Application.DTO.DonationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Donation
{
    public interface IDonationService
    {
        Task<int> RegisterDonationAsync(RegisterDonationDto dto);
        Task CheckInDonorAsync(int registrationId, Guid staffId);
        Task<bool> ConductMedicalCheckupAsync(int historyId, bool isHealthy, Guid staffId);
        Task<bool> CollectBloodAsync(int historyId, float volume, bool isQualified, Guid staffId);
    }
}
