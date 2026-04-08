using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Categories.Commands;

// Request chứa danh sách các danh mục cần tạo
public record BulkCreateCategoryCommand(List<CategoryEntry> Categories) : IRequest<List<Guid>>;

public record CategoryEntry(string Name, string? ColorHex);

public class BulkCreateCategoryCommandHandler : IRequestHandler<BulkCreateCategoryCommand, List<Guid>>
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public BulkCreateCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow, ICurrentUserService user)
    {
        _repo = repo;
        _uow = uow;
        _user = user;
    }

    public async Task<List<Guid>> Handle(BulkCreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.Categories == null || !request.Categories.Any())
            return new List<Guid>();

        var userId = _user.UserId;
        var newCategories = request.Categories.Select(c => new Category
        {
            Id = Guid.NewGuid(),
            Name = c.Name,
            ColorHex = c.ColorHex,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow
        }).ToList();

        // Sử dụng giao dịch để đảm bảo an toàn
        await _uow.BeginTransactionAsync(cancellationToken);
        try
        {
            _repo.AddRange(newCategories);
            await _uow.CommitTransactionAsync(cancellationToken);

            return newCategories.Select(c => c.Id).ToList();
        }
        catch
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}