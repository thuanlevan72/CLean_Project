using Application.IRepository.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Postgres.IRepository
{
    public interface ITodoRepository: IBaseRepository<TodoItem>
    {

    }
}
