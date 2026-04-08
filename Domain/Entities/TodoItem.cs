using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class TodoItem : BaseEntity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PriorityLevel Priority { get; set; }
    public TodoStatus Status { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public Guid UserId { get; set; }

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid? ParentTaskId { get; set; }
    public TodoItem? ParentTask { get; set; }

    public ICollection<TodoItem> SubTasks { get; set; } = new List<TodoItem>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}