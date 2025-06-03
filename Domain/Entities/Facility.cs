using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Facility
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        //public Microsoft.SqlServer.Types.SqlGeography? Longtitude { get; set; }
        //public Microsoft.SqlServer.Types.SqlGeography? Latitude { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Description { get; set; }
        public string? OpeningDay { get; set; }
        public string? ClosingDay { get; set; }
        public TimeSpan? OpeningHour { get; set; }
        public TimeSpan? ClosingHour { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
