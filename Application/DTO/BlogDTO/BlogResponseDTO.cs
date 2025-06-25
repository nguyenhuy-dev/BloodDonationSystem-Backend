using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BlogDTO
{
    public class BlogResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Author { get; set; }

        public bool IsActived { get; set; }
    }
}
