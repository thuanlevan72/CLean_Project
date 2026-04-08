using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Tags.Commands;

public record BulkCreateTagCommand(List<string> TagNames) : IRequest<List<int>>;

public class BulkCreateTagCommandHandler : IRequestHandler<BulkCreateTagCommand, List<int>>
{
    private readonly ITagRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public BulkCreateTagCommandHandler(ITagRepository repo, IUnitOfWork uow, ICurrentUserService user)
    {
        _repo = repo;
        _uow = uow;
        _user = user;
    }

    public async Task<List<int>> Handle(BulkCreateTagCommand request, CancellationToken cancellationToken)
    {
        if (request.TagNames == null || !request.TagNames.Any())
            return new List<int>();

        var userId = _user.UserId;
        var newTags = request.TagNames.Select(name => new Tag
        {
            Name = name,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow
        }).ToList();

        await _uow.BeginTransactionAsync(cancellationToken);
        try
        {
            _repo.AddRange(newTags);
            await _uow.CommitTransactionAsync(cancellationToken);

            return newTags.Select(t => t.Id).ToList();
        }
        catch
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}