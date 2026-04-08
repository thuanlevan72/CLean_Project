using TodoApi.DTOs;

namespace TodoApi.Services;

public interface ITodoService
{
  Task<IEnumerable<TodoDto>> GetAllTodosAsync();
  Task<TodoDto?> GetTodoByIdAsync(int id);
  Task<TodoDto> CreateTodoAsync(CreateTodoDto dto);
  Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto dto);
  Task<bool> DeleteTodoAsync(int id);
  Task<TodoDto?> ToggleTodoAsync(int id);
}
