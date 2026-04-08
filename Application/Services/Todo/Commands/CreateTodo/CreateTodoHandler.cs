//using Application.Dtos;
//using AutoMapper;
//using MediatR;
//using Application.IRepository.Base;
//namespace Application.Services.Todo.Commands.CreateTodo
//{
//    public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoDto>
//    {
//        private readonly IUnitOfWork _unit; // Interface từ lớp Application
//        private readonly IMapper _mapper;

//        // Dependency Injection tại Handler
//        public CreateTodoCommandHandler(IUnitOfWork unit, IMapper mapper)
//        {
//            _unit = unit;
//            _mapper = mapper;
//        }

//        public async Task<TodoDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                await _unit.BeginTransactionAsync();
//                //var todo = _mapper.Map<TodoApi.Models.Todo>(request.CreateTodoDto);
//                var todo = new TodoApi.Models.Todo
//                {
//                    Text = request.CreateTodoDto.Text,
//                };
//                todo.Id  = Guid.NewGuid();
//                todo.CategoryId = Guid.Parse("b6c27b5c-9849-4ce4-a5b4-b3d2a55daa25");
//                await _unit.TodoRepository.AddAsync(todo);
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
//            catch(Exception ex)
//            {
//                await _unit.RollbackTransactionAsync();
//                throw;
//            }
//        }
//    }
//}
