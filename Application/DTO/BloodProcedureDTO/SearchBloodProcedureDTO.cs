using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BloodProcedureDTO
{
    public class SearchBloodProcedureDTO
    {
        public int Id { get; set; }
        public int DonationRegisId { get; set; }
        public float Volume { get; set; }
        public string FullName { get; set; }
        public string? BloodTypeName { get; set; }
        public DateTime PerformedAt { get; set; }
        public bool? IsQualified { get; set; }
        public string Phone { get; set; }
    }
}
