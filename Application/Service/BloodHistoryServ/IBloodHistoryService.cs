using Application.DTO.BloodHistoryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BloodHistoryServ
{
    public interface IBloodHistoryService
    {
        Task<List<UnifiedBloodHistory>> GetBloodRegistraionHistoryAsync();
        Task<List<DonationHistory>> GetDonationHistoryAsync();

        Task<bool> UpdateAvailableDateVolunteerAsync(int id, UpdateAvailableDateDTO dto);
    }
}
