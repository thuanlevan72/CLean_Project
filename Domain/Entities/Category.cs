using Domain.Entities.Base;

namespace Domain.Entities;

public class Category : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? ColorHex { get; set; }

    public Guid UserId { get; set; }

    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}