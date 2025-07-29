using Application.DTO.EventsDTO;
using Application.DTO;
using Application.DTO.ReportDTO;
using Infrastructure.Helper;
using Infrastructure.Repository.ReportRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.BloodRegistrationRepo;

namespace Application.Service.ReportServ
{
    public class ReportService(IReportRepository _repo, IEventRepository _eventRepository,
                                IBloodRegistrationRepository _bloodRegisRepo) : IReportService
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

        public async Task<ApiResponse<PaginatedResult<EventForDashAdminDTO>>> GetEventsForDashboardAdminAsync(int pageNumber, int pageSize)
        {
            ApiResponse<PaginatedResult<EventForDashAdminDTO>> apiResponse = new()
            {
                IsSuccess = true,
                Message = "Events retrieved successfully.",
                Data = new PaginatedResult<EventForDashAdminDTO>
                {
                    Items = new List<EventForDashAdminDTO>(),
                    TotalItems = (await _eventRepository.GetAllEventNotPagedAsync()).Count(),
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }
            };

            var events = (await _eventRepository.GetAllEventNotPagedAsync())
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

            var eventDTOs = events.Select(e => new EventForDashAdminDTO
            {
                Id = e.Id,
                EventTime = e.EventTime,
                Address = e.Facility?.Address ?? "Unknown",
                BloodRegisCount = _bloodRegisRepo.GetByEventAsync(e.Id).Result.Count(),
                IsExpired = e.IsExpired,
                IsUrgent = e.IsUrgent,
                BloodType = e.BloodType?.Type ?? null,
                BloodComponent = e.BloodComponent?.ToString() ?? null,
                BloodTypeId = e.BloodTypeId ?? null
            }).ToList();

            foreach (var eventDTO in eventDTOs)
            {
                // Xét số Blood Registrations mà thành công, trong đó đối với urgent event thì cần phải cùng nhóm máu 
                if (eventDTO.IsUrgent == false)
                    eventDTO.SuccessfulBloodRegisCount = (await _bloodRegisRepo.GetByEventAsync(eventDTO.Id)).Count(br => br.IsApproved == true &&
                                                                                            br.BloodProcedure.IsQualified == true);
                else
                    eventDTO.SuccessfulBloodRegisCount = (await _bloodRegisRepo.GetByEventAsync(eventDTO.Id)).Count(br => br.IsApproved == true &&
                                                                                            br.BloodProcedure.IsQualified == true &&
                                                                                            br.BloodProcedure.BloodTypeId == eventDTO.BloodTypeId);

                if (eventDTO.EventTime > DateOnly.FromDateTime(DateTime.Now))
                    eventDTO.Status = "Sắp diễn ra";
                else if (eventDTO.EventTime == DateOnly.FromDateTime(DateTime.Now))
                    eventDTO.Status = "Đang diễn ra";
                else
                    eventDTO.Status = "Đã hoàn thành";
            }

            apiResponse.Data.Items = eventDTOs.ToList();

            return apiResponse;
        }
    }
}
