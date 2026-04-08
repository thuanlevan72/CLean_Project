using Application.Interfaces;
using Domain.Repositories;
using MediatR;

namespace Application.Features.TodoItems.Commands;

public record DeleteTodoCommand(Guid Id) : IRequest;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand>
{
    private readonly ITodoItemRepository _todoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTodoCommandHandler(ITodoItemRepository todoRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _todoRepository = todoRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAndUserIdAsync(request.Id, _currentUserService.UserId, cancellationToken);
        if (todo == null) throw new Exception("Không tìm thấy công việc.");

        _todoRepository.Delete(todo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}