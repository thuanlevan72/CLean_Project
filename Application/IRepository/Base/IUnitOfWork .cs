using Application.Postgres.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.IRepository.Base
{
    public interface IUnitOfWork: IDisposable
    {

        #region Repositories
        
        ITodoRepository TodoRepository { get; }
        #endregion


        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task<int> SaveChangesAsync();
    }
}
