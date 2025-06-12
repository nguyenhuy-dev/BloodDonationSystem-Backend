using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BloodRegistration
{
    public class BloodRegistrationRequest
    {
        public string? Address { get; set; }
        public DateTime? LastDonation { get; set; } = null;
        public int? BloodTypeId { get; set; } = null;
        public string? Phone { get; set; }
        public string? Gmail { get; set; }
        public int EventId { get; set; } // Lấy từ giao diện khi nhấn "Đăng ký"
        public Guid MemberId { get; set; }
    }
}
