using Application.Interfaces;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.TodoItems.Commands;

public record UpdateTodoCommand(Guid Id, TodoStatus NewStatus) : IRequest;

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand>
{
    private readonly ITodoItemRepository _todoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTodoCommandHandler(ITodoItemRepository todoRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _todoRepository = todoRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAndUserIdAsync(request.Id, _currentUserService.UserId, cancellationToken);
        if (todo == null) throw new Exception("Không tìm thấy công việc.");

        todo.Status = request.NewStatus;
        if (request.NewStatus == TodoStatus.Completed) todo.CompletedAt = DateTimeOffset.UtcNow;

        _todoRepository.Update(todo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}