using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Postgres.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Repository;

public class CategoryRepository : GenericRepository<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);
    }

    public async Task<List<Category>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}