using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
  Task<IEnumerable<Todo>> GetAllAsync();
  Task<Todo?> GetByIdAsync(int id);
  Task<Todo> CreateAsync(Todo todo);
  Task<Todo> UpdateAsync(Todo todo);
  Task<bool> DeleteAsync(int id);
}
