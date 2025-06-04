using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.DonationDTO
{
    public class RegisterDonationDto
    {
        public Guid UserId { get; set; }
        public int EventId { get; set; }
        public bool IsVolunteer { get; set; }
    }
}
