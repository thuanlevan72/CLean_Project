using Application.IRepository.Base;
using Common.QueryParams;
using Domain.Models.Base;
using Infrastructure.Postgres.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Infrastructure.IRepository.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, new()
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        #region 

        #endregion 

        #region Query

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate,
                                       params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        #endregion

        #region Paging

        public async Task<PagedResult<T>> GetPagedAsync(QueryParams param)
        {
            var query = _dbSet.AsQueryable();

            // Search (optional - nếu có property Name)
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                // NOTE: cần customize theo entity thực tế
            }

            // Sort
            if (!string.IsNullOrEmpty(param.SortBy))
            {
                query = param.IsDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, param.SortBy))
                    : query.OrderBy(e => EF.Property<object>(e, param.SortBy));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((param.PageIndex - 1) * param.PageSize)
                .Take(param.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = param.PageIndex,
                PageSize = param.PageSize
            };
        }

        #endregion

        #region Command

        public async Task AddAsync(T entity)
        {
          await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task SoftDeleteAsync(T entity)
        {
            if (entity is ISoftDelete softDelete)
            {
                softDelete.SoftDelete = DateTimeOffset.UtcNow;
                _dbSet.Update(entity);
                await Task.CompletedTask;
            }
              
        }

        #endregion

    }
}
