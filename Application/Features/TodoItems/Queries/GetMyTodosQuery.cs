using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Features.TodoItems.Queries;

public record GetMyTodosQuery() : IRequest<List<TodoDto>>;

public class GetMyTodosQueryHandler : IRequestHandler<GetMyTodosQuery, List<TodoDto>>
{
    private readonly ITodoItemRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMyTodosQueryHandler(ITodoItemRepository todoRepository, ICurrentUserService currentUserService, IMapper mapper)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<List<TodoDto>> Handle(GetMyTodosQuery request, CancellationToken cancellationToken)
    {
        var todos = await _todoRepository.GetAllByUserIdAsync(_currentUserService.UserId, cancellationToken);
        return _mapper.Map<List<TodoDto>>(todos); // Sử dụng AutoMapper cực kỳ gọn gàng
    }
}