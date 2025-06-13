using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BloodProcedureDTO
{
    public class BloodCollectionRequest
    {
        public float Volume { get; set; } 
        public string? Description { get; set; } = null;
        public Guid PerformedBy { get; set; } 
        public int BloodRegistrationId { get; set; }
    }
}
