namespace TodoApi.DTOs;

public class TodoDto
{
  public int Id { get; set; }
  public string Text { get; set; } = string.Empty;
  public bool Completed { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
