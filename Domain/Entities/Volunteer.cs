using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Volunteer
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime StartVolunteerDate { get; set; }
        public DateTime EndVolunteerDate { get; set; }
        public bool IsExpired { get; set; }

        public Guid MemberId { get; set; }

        [ForeignKey("MemberId")]
        public virtual User Member { get; set; }

        public virtual BloodRegistration BloodRegistration { get; set; }
    }
}
