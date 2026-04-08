using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
  private readonly ITodoRepository _repository;

  public TodoService(ITodoRepository repository)
  {
    _repository = repository;
  }

  public async Task<IEnumerable<TodoDto>> GetAllTodosAsync()
  {
    var todos = await _repository.GetAllAsync();
    return todos.Select(MapToDto);
  }

  public async Task<TodoDto?> GetTodoByIdAsync(int id)
  {
    var todo = await _repository.GetByIdAsync(id);
    return todo == null ? null : MapToDto(todo);
  }

  public async Task<TodoDto> CreateTodoAsync(CreateTodoDto dto)
  {
    var todo = new Todo
    {
      Text = dto.Text,
      Completed = false,
      CreatedAt = DateTime.UtcNow
    };

    var created = await _repository.CreateAsync(todo);
    return MapToDto(created);
  }

  public async Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto dto)
  {
    var todo = await _repository.GetByIdAsync(id);
    if (todo == null) return null;

    if (dto.Text != null) todo.Text = dto.Text;
    if (dto.Completed.HasValue) todo.Completed = dto.Completed.Value;

    var updated = await _repository.UpdateAsync(todo);
    return MapToDto(updated);
  }

  public async Task<bool> DeleteTodoAsync(int id)
  {
    return await _repository.DeleteAsync(id);
  }

  public async Task<TodoDto?> ToggleTodoAsync(int id)
  {
    var todo = await _repository.GetByIdAsync(id);
    if (todo == null) return null;

    todo.Completed = !todo.Completed;
    var updated = await _repository.UpdateAsync(todo);
    return MapToDto(updated);
  }

  private static TodoDto MapToDto(Todo todo)
  {
    return new TodoDto
    {
      Id = todo.Id,
      Text = todo.Text,
      Completed = todo.Completed,
      CreatedAt = todo.CreatedAt,
      UpdatedAt = todo.UpdatedAt
    };
  }
}
