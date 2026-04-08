using Domain.Entities.Base;
using Domain.Repositories;
using Infrastructure.Postgres.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Repository;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : BaseEntity<TId>
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
    }

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual void Add(TEntity entity) => _dbSet.Add(entity);
    public virtual void AddRange(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);
    public virtual void Update(TEntity entity) => _dbSet.Update(entity);
    public virtual void UpdateRange(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);
    public virtual void Delete(TEntity entity) => _dbSet.Remove(entity);
    public virtual void DeleteRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
}