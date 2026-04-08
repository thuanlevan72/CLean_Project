namespace Application.Events;

public class TodoCreatedEvent
{
    public Guid TodoId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}