using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Blog
{
    [Key]
    public int Id { get; set; }
    [StringLength(200)]
    public string Title { get; set; }
    public string Content { get; set; }
    
    public DateTime CreateAt { get; set; }
    public DateTime? LastUpdate { get; set; }
    public bool IsActived { get; set; }

    public Guid AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public virtual User Author { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
}
