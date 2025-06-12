using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.GoogleDTO
{
    public class UpdateGoogleLogin
    {
        //public string Credential { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Gmail { get; set; }
        public string Password { get; set; }
        public string BloodType { get; set; }
        public DateOnly Dob { get; set; }
        public bool Gender { get; set; }
    }
}
