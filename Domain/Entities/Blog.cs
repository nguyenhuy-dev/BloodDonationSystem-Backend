using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime LastUpdate { get; set; }
    public string Content { get; set; } = null!;
    public bool Status { get; set; }

    public User Author { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
