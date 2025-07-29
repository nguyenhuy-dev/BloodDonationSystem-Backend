using Application.DTO.EventsDTO;
using Application.DTO;
using Application.DTO.ReportDTO;
using Infrastructure.Helper;
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

        Task<ApiResponse<PaginatedResult<EventForDashAdminDTO>>> GetEventsForDashboardAdminAsync(int pageNumber, int pageSize);
    }
}
