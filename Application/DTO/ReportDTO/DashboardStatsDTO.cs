using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.ReportDTO
{
    public class DashboardStatsDTO
    {
        public int CurrentViewers { get; set; }
        public int BloodUnitsInStock { get; set; }
        public int MonthlyEvents { get; set; }
        public int YearlyDonors { get; set; }
        public int TotalUsers { get; set; }
        public WeeklyChangeDTO WeeklyChanges { get; set; } = new();
    }
}
