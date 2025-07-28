using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.ReportRepository
{
    public class ReportRepository(BloodDonationSystemContext _context) : IReportRepository
    {
        public async Task<List<BloodInventory>> GetBloodStockByTypeAsync()
        {
            return await _context.BloodInventories
                .Include(bi => bi.BloodType)
                .ToListAsync();
        }

        public async Task<List<BloodRegistration>> GetDonationActivityThisYearAsync()
        {
            return await _context.BloodRegistrations
                .Where(br => br.CreateAt.Year == TimeHelper.NowVietnam.Year)
                .ToListAsync();
        }

        public async Task<int> GetMonthlyEventsAsync()
        {
            return await _context.Events
                .CountAsync(e => e.EventTime.Month == TimeHelper.NowVietnam.Month && e.EventTime.Year == TimeHelper.NowVietnam.Year);
        }
        public async Task<int> GetLastMonthEventsAsync()
        {
            var now = TimeHelper.NowVietnam;
            var lastMonth = now.Month == 1 ? 12 : now.Month - 1;
            var lastYear = now.Month == 1 ? now.Year - 1 : now.Year;
            return await _context.Events
                .CountAsync(e => e.EventTime.Month == lastMonth && e.EventTime.Year == lastYear);
        }

        public async Task<int> GetNewUsersThisMonthAsync()
        {
            return await _context.Users
                .CountAsync(u => u.CreateAt.Month == TimeHelper.NowVietnam.Month && u.CreateAt.Year == TimeHelper.NowVietnam.Year);
        }
        public async Task<int> GetNewUsersLastMonthAsync()
        {
            var now = TimeHelper.NowVietnam;
            var lastMonth = now.Month == 1 ? 12 : now.Month - 1;
            var lastYear = now.Month == 1 ? now.Year - 1 : now.Year;
            return await _context.Users
                .CountAsync(u => u.CreateAt.Month == lastMonth && u.CreateAt.Year == lastYear);
        }

        public async Task<int> GetTotalBloodUnitsAsync()
        {
            return await _context.BloodInventories.CountAsync();
        }
        public async Task<int> GetTotalBloodUnitsThisMonthAsync()
        {
            return await _context.BloodInventories
                .CountAsync(bi => bi.CreateAt.Month == TimeHelper.NowVietnam.Month && bi.CreateAt.Year == TimeHelper.NowVietnam.Year);
        }
        public async Task<int> GetTotalBloodUnitsLastMonthAsync()
        {
            var now = TimeHelper.NowVietnam;
            var lastMonth = now.Month == 1 ? 12 : now.Month - 1;
            var lastYear = now.Month == 1 ? now.Year - 1 : now.Year;
            return await _context.BloodInventories
                .CountAsync(bi => bi.CreateAt.Month == lastMonth && bi.CreateAt.Year == lastYear);
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        //public Task<WeeklyChangeDTO> GetWeeklyChangesAsync()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<int> GetYearlyDonorsAsync()
        {
            return await _context.BloodRegistrations
                .CountAsync(br => br.CreateAt.Year == TimeHelper.NowVietnam.Year);
        }
        public async Task<int> GetLastYearDonorsAsync()
        {
            var now = TimeHelper.NowVietnam;
            var lastYear = now.Month == 1 ? now.Year - 1 : now.Year;
            return await _context.BloodRegistrations
                .CountAsync(br => br.CreateAt.Year == lastYear);
        }
    }
}
