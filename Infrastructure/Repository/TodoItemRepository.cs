using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Postgres.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Repository;

public class TodoItemRepository : GenericRepository<TodoItem, Guid>, ITodoItemRepository
{
    public TodoItemRepository(AppDbContext context) : base(context) { }

    // Implement hàm vừa thêm
    public async Task<TodoItem?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);
    }

    public async Task<TodoItem?> GetByIdWithDetailsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Category)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);
    }

    public async Task<List<TodoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}