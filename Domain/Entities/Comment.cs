using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int BlogId { get; set; }
    public string Text { get; set; } = null!;
    public bool Status { get; set; }

    public User User { get; set; }
    public Blog Blog { get; set; }
}
