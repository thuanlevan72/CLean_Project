using Application.Postgres.IRepository;
using Domain.Entities;
using Infrastructure.IRepository.Base;
using Infrastructure.Postgres.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Postgres.Repository
{
    public class TodoRepository : BaseRepository<TodoItem>, ITodoRepository
    {
        public TodoRepository(AppDbContext context) : base(context)
        {
        }

    }
}
