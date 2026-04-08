using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Features.TodoItems.Queries;

public record GetTodoByIdQuery(Guid Id) : IRequest<TodoDto>;

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoDto>
{
    private readonly ITodoItemRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetTodoByIdQueryHandler(ITodoItemRepository todoRepository, ICurrentUserService currentUserService, IMapper mapper)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TodoDto> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdWithDetailsAsync(request.Id, _currentUserService.UserId, cancellationToken);
        if (todo == null) throw new Exception("Không tìm thấy công việc.");

        return _mapper.Map<TodoDto>(todo);
    }
}