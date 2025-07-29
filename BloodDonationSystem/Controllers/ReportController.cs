using Application.Service.ReportServ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [ApiController]
    public class ReportController(IReportService _serv) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("api/reports/stats")]
        public async Task<IActionResult> GetDashboardStatsReport()
        {
            var result = await _serv.GetDashboardStatsReportAsync();
            return Ok(new
            {
                IsSuccess = true,
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/reports/blood-stock")]
        public async Task<IActionResult> GetDashboardBloodStockReport()
        {
            var result = await _serv.GetDashboardBloodStockReportAsync();
            return Ok(new
            {
                IsSuccess = true,
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/reports/activities")]
        public async Task<IActionResult> GetDashboardDonationActivitiesReport()
        {
            var result = await _serv.GetDashboardDonorsReportAsync();
            return Ok(new
            {
                IsSuccess = true,
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/reports/events")]
        public async Task<IActionResult> GetDashboardEventsReport(int pageNumber, int pageSize)
        {
            var apiResponse = await _serv.GetEventsForDashboardAdminAsync(pageNumber, pageSize);

            return Ok(apiResponse);
        }
    }
}
