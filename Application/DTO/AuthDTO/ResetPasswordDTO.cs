using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.AuthDTO
{
    public class ResetPasswordDTO
    {
        public string Phone { get; set; }
        public string NewPassword { get; set; }
    }
}
