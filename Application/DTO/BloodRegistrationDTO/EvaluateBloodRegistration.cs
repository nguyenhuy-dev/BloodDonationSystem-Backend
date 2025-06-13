using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BloodRegistrationDTO
{
    public class EvaluateBloodRegistration
    {
        public RegistrationStatus Status { get; set; }
        public Guid StaffId { get; set; }
    }
}
