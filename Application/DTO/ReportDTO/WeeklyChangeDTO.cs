using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.ReportDTO
{
    public class WeeklyChangeDTO
    {
        public PercentageTrendDTO Viewers { get; set; }
        public PercentageTrendDTO BloodUnits { get; set; }
        public PercentageTrendDTO Events { get; set; }
        public PercentageTrendDTO Donors { get; set; }
    }
}
