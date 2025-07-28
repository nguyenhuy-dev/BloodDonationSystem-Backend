using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.ReportRepository
{
    public interface IReportRepository
    {
        Task<int> GetTotalUsersAsync();

        Task<int> GetNewUsersThisMonthAsync();
        Task<int> GetNewUsersLastMonthAsync();

        Task<int> GetTotalBloodUnitsAsync();
        Task<int> GetTotalBloodUnitsThisMonthAsync();
        Task<int> GetTotalBloodUnitsLastMonthAsync();

        Task<int> GetMonthlyEventsAsync();
        Task<int> GetLastMonthEventsAsync();

        Task<int> GetYearlyDonorsAsync();
        Task<int> GetLastYearDonorsAsync();

        Task<List<BloodInventory>> GetBloodStockByTypeAsync();

        Task<List<BloodRegistration>> GetDonationActivityThisYearAsync();
        //Task<WeeklyChangeDTO> GetWeeklyChangesAsync();

    }
}
