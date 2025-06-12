using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Comment
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Text { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public bool? IsLegit { get; set; }

    public Guid? StaffId { get; set; }
    public Guid MemberId { get; set; }
    public int BlogId { get; set; }

    [ForeignKey("MemberId")]
    public virtual User Member { get; set; }

    [ForeignKey("StaffId")]
    public virtual User Staff { get; set; }

    [ForeignKey("BlogId")]
    public virtual Blog Blog { get; set; }
}
