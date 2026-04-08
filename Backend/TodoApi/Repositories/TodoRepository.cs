using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoRepository : ITodoRepository
{
  private readonly AppDbContext _context;

  public TodoRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Todo>> GetAllAsync()
  {
    return await _context.Todos.OrderByDescending(t => t.CreatedAt).ToListAsync();
  }

  public async Task<Todo?> GetByIdAsync(int id)
  {
    return await _context.Todos.FindAsync(id);
  }

  public async Task<Todo> CreateAsync(Todo todo)
  {
    _context.Todos.Add(todo);
    await _context.SaveChangesAsync();
    return todo;
  }

  public async Task<Todo> UpdateAsync(Todo todo)
  {
    todo.UpdatedAt = DateTime.UtcNow;
    _context.Entry(todo).State = EntityState.Modified;
    await _context.SaveChangesAsync();
    return todo;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var todo = await _context.Todos.FindAsync(id);
    if (todo == null) return false;

    _context.Todos.Remove(todo);
    await _context.SaveChangesAsync();
    return true;
  }
}
