using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.HealthProcedureDTO
{
    public class HealthProcedureRequest
    {
        public int Pressure { get; set; }
        public float Temperature { get; set; }
        public float Hb { get; set; }
        public bool HBV { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public bool IsHealth { get; set; }
        public string? Description { get; set; }
        public Guid PerformedBy { get; set; }
        public int BloodRegistrationId { get; set; } // Lấy từ Frontend
    }
}
