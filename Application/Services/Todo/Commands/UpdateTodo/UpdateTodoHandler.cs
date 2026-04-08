//using Application.Dtos;
//using Application.IRepository.Base;
//using Application.Postgres.IRepository;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Application.Services.Todo.Commands.UpdateTodo
//{
//    public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, TodoDto?>
//    {
//        private readonly IUnitOfWork _unit;

//        public UpdateTodoHandler(IUnitOfWork unit)
//        {
//            _unit = unit;
//        }

//        public async Task<TodoDto?> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                await _unit.BeginTransactionAsync();
//                var todo = await _unit.TodoRepository.GetByIdAsync(request.Id);
//                if (todo == null) return null;
//                todo.Completed = request?.Dto?.Completed ?? todo.Completed;
//                todo.Text = request?.Dto?.Text ?? todo.Text;
//                _unit.TodoRepository.Update(todo);
//                await _unit.SaveChangesAsync();
//                await _unit.CommitTransactionAsync();
//                return new TodoDto
//                {
//                    Id = todo.Id.GetHashCode(),
//                    Text = todo.Text,
//                    Completed = todo.Completed,
//                    CreatedAt = todo.CreatedAt,
//                    UpdatedAt = todo.UpdatedAt
//                };
//            }
//            catch (Exception ex) {
//                await _unit.RollbackTransactionAsync();
//                throw;
//            }
            
            
//        }
//    }
//}
