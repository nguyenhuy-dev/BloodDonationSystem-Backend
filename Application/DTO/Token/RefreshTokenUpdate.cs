using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Token
{
    public class RefreshTokenUpdate
    {
        public string RefreshToken { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
    }
}
