using Application.DTO.ReportDTO;
using Infrastructure.Repository.ReportRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.ReportServ
{
    public class ReportService(IReportRepository _repo) : IReportService
    {
        public async Task<List<BloodStockDTO>> GetDashboardBloodStockReportAsync()
        {
            var bloodList = await _repo.GetBloodStockByTypeAsync();

            var stock = bloodList
                .GroupBy(bl => bl.BloodType)
                .Select(bg => new BloodStockDTO
                {
                    Type = bg.Key.Type,
                    Quantity = bg.Count()
                }).ToList();

            return stock;
        }

        public async Task<List<DonationActivitiesDTO>> GetDashboardDonorsReportAsync()
        {
            var donationList = await _repo.GetDonationActivityThisYearAsync();
            var donation = donationList
                .GroupBy(dl => dl.CreateAt.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Donations = g.Count()
                });

            var fullData = Enumerable.Range(1, 12).Select(m => new DonationActivitiesDTO
            {
                Month = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(m).ToUpper(), // JAN, FEB,...
                Donations = donation.FirstOrDefault(d => d.Month == m)?.Donations ?? 0
            }).ToList();

            return fullData;
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsReportAsync()
        {
            var stats = new DashboardStatsDTO
            {
                CurrentViewers = await _repo.GetNewUsersThisMonthAsync(),
                BloodUnitsInStock = await _repo.GetTotalBloodUnitsAsync(),
                MonthlyEvents = await _repo.GetMonthlyEventsAsync(),
                YearlyDonors = await _repo.GetYearlyDonorsAsync(),
                TotalUsers = await _repo.GetTotalUsersAsync(),
                WeeklyChanges =
                {
                    Viewers = PercentageTrend(await _repo.GetNewUsersThisMonthAsync(), await _repo.GetNewUsersLastMonthAsync()),
                    BloodUnits = PercentageTrend(await _repo.GetTotalBloodUnitsThisMonthAsync(), await _repo.GetTotalBloodUnitsLastMonthAsync()),
                    Events = PercentageTrend(await _repo.GetMonthlyEventsAsync(), await _repo.GetLastMonthEventsAsync()),
                    Donors = PercentageTrend(await _repo.GetYearlyDonorsAsync(), await _repo.GetLastYearDonorsAsync())
                }
            };

            return stats;
        }
        private PercentageTrendDTO PercentageTrend(int current, int previous)
        {
            if (previous == 0)
                return new PercentageTrendDTO { Value = "N/A", Trend = "up" };

            var rate = ((double)(current - previous) / previous) * 100;
            string trend = rate >= 0 ? "up" : "down";
            return new PercentageTrendDTO
            {
                Value = $"{rate:+0.00;-0.00}%",
                Trend = trend
            };
        }
    }
}
