using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, string? ColorHex) : IRequest<Guid>;
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public CreateCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow, ICurrentUserService user)
    { _repo = repo; _uow = uow; _user = user; }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category { Name = request.Name, ColorHex = request.ColorHex, UserId = _user.UserId };
        _repo.Add(category);
        await _uow.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}

public record UpdateCategoryCommand(Guid Id, string Name, string? ColorHex) : IRequest;
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public UpdateCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow, ICurrentUserService user)
    { _repo = repo; _uow = uow; _user = user; }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repo.GetByIdAsync(request.Id, _user.UserId, cancellationToken);
        if (category == null) throw new Exception("Không tìm thấy danh mục.");

        category.Name = request.Name;
        category.ColorHex = request.ColorHex;
        _repo.Update(category);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteCategoryCommand(Guid Id) : IRequest;
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public DeleteCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow, ICurrentUserService user)
    { _repo = repo; _uow = uow; _user = user; }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repo.GetByIdAsync(request.Id, _user.UserId, cancellationToken);
        if (category == null) throw new Exception("Không tìm thấy danh mục.");

        _repo.Delete(category);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}