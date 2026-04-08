
using Domain.Entities.Base;

namespace Domain.Entities;

public class Tag : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}