using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.TodoItems.Commands;

public record CreateTodoCommand(
    string Title,
    string? Description,
    PriorityLevel Priority,
    DateTimeOffset? DueDate,
    Guid? CategoryId) : IRequest<Guid>;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Guid>
{
    private readonly ITodoItemRepository _todoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDistributedLockService _lockService;

    public CreateTodoCommandHandler(
        ITodoItemRepository todoRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDistributedLockService lockService)
    {
        _todoRepository = todoRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _lockService = lockService;
    }

    public async Task<Guid> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedAccessException("Bạn chưa đăng nhập.");


        var userId = _currentUserService.UserId;
        var lockToken = Guid.NewGuid().ToString();
        // Khóa lại theo UserId trong 5 giây. Đứa nào click đúp sẽ bị văng ở request thứ 2.
        bool isLocked = await _lockService.AcquireLockAsync($"create_todo_{userId}", lockToken, TimeSpan.FromSeconds(5));

        if (!isLocked) throw new Exception("Bạn đang thao tác quá nhanh, vui lòng chờ!");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, userId, cancellationToken);
                if (category == null) throw new Exception("Danh mục không tồn tại.");
            }

            var todo = new TodoItem
            {
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Status = TodoStatus.Todo,
                DueDate = request.DueDate,
                CategoryId = request.CategoryId
            };

            _todoRepository.Add(todo);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return todo.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}