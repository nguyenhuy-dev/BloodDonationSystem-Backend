using Application.DTO.ReportDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.ReportServ
{
    public interface IReportService
    {
        Task<DashboardStatsDTO> GetDashboardStatsReportAsync();
        Task<List<BloodStockDTO>> GetDashboardBloodStockReportAsync();
        Task<List<DonationActivitiesDTO>> GetDashboardDonorsReportAsync();
    }
}
