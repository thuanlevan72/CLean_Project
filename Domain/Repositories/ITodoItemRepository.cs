using Domain.Entities;

namespace Domain.Repositories;

public interface ITodoItemRepository : IGenericRepository<TodoItem, Guid>
{
    // Thêm hàm này để query bảo mật theo User
    Task<TodoItem?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<TodoItem?> GetByIdWithDetailsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<List<TodoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}