using Common.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Application.IRepository.Base
{
    public interface IBaseRepository<T>
    {
        Task<T?> GetAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes);


        Task<T?> GetByIdAsync(Guid id);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        // Paging
        Task<PagedResult<T>> GetPagedAsync(QueryParams param);

        // Command
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);

        Task SoftDeleteAsync(T entity);
    }
}
