using System;
using System.Collections.Generic;

namespace DatabaseFristPostgres.GeneratedCode;

public partial class Todo
{
    public Guid Id { get; set; }

    public string Text { get; set; } = null!;

    public bool Completed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? SoftDelete { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;
}
