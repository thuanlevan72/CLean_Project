using System;
using System.Collections.Generic;

namespace DatabaseFristPostgres.GeneratedCode;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? SoftDelete { get; set; }

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
