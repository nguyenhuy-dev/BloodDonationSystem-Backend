using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.UserDTO
{
    public class ProfileDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Gmail { get; set; }
        public string BloodType { get; set; }
        public DateOnly? Dob { get; set; }
        public bool? Gender { get; set; }
        public string Role { get; set; }
    }
}
