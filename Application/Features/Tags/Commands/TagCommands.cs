using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Tags.Commands;

public record CreateTagCommand(string Name) : IRequest<int>;
public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
{
    private readonly ITagRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public CreateTagCommandHandler(ITagRepository repo, IUnitOfWork uow, ICurrentUserService user)
    { _repo = repo; _uow = uow; _user = user; }

    public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = new Tag { Name = request.Name, UserId = _user.UserId };
        _repo.Add(tag);
        await _uow.SaveChangesAsync(cancellationToken);
        return tag.Id;
    }
}

public record UpdateTagCommand(int Id, string Name) : IRequest;
public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand>
{
    private readonly ITagRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public UpdateTagCommandHandler(ITagRepository repo, IUnitOfWork uow, ICurrentUserService user)
    { _repo = repo; _uow = uow; _user = user; }

    public async Task Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _repo.GetByIdAsync(request.Id, _user.UserId, cancellationToken);
        if (tag == null) throw new Exception("Không tìm thấy thẻ.");

        tag.Name = request.Name;
        _repo.Update(tag);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteTagCommand(int Id) : IRequest;
public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly ITagRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public DeleteTagCommandHandler(ITagRepository repo, IUnitOfWork uow, ICurrentUserService user)
    { _repo = repo; _uow = uow; _user = user; }

    public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _repo.GetByIdAsync(request.Id, _user.UserId, cancellationToken);
        if (tag == null) throw new Exception("Không tìm thấy thẻ.");

        _repo.Delete(tag);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}