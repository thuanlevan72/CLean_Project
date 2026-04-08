using Domain.Entities;

namespace Domain.Repositories;

public interface ICategoryRepository : IGenericRepository<Category, Guid>
{
    Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<List<Category>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}