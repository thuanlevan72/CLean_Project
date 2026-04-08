using Domain.Entities;

namespace Domain.Repositories;

public interface ITagRepository : IGenericRepository<Tag, int>
{
    Task<Tag?> GetByIdAsync(int id, Guid userId, CancellationToken cancellationToken = default);
    Task<List<Tag>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}