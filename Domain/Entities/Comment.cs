using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Comment
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int BlogId { get; set; }

    public string Text { get; set; } = null!;

    public bool Status { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
