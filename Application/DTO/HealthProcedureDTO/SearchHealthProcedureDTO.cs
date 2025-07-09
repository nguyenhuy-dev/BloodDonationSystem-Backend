using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.HealthProcedureDTO
{
    public class SearchHealthProcedureDTO
    {
        public int Id { get; set; }
        public bool IsHealth { get; set; }
        public DateTime PerformedAt { get; set; }
        public string FullName { get; set; }
        public string? BloodTypeName { get; set; }
        public int BloodRegisId { get; set; }
        public string Phone { get; set; }
    }
}
