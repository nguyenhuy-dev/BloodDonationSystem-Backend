using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.UserDTO
{
    public class UpdateUserDTO
    {
        public DateOnly? Dob { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
