using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Tags.Queries;

public record GetMyTagsQuery() : IRequest<List<TagDto>>;
public class GetMyTagsQueryHandler : IRequestHandler<GetMyTagsQuery, List<TagDto>>
{
    private readonly ITagRepository _repo;
    private readonly ICurrentUserService _user;
    private readonly IMapper _mapper;

    public GetMyTagsQueryHandler(ITagRepository repo, ICurrentUserService user, IMapper mapper)
    { _repo = repo; _user = user; _mapper = mapper; }

    public async Task<List<TagDto>> Handle(GetMyTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _repo.GetAllByUserIdAsync(_user.UserId, cancellationToken);
        return _mapper.Map<List<TagDto>>(tags);
    }
}

public record GetTagByIdQuery(int Id) : IRequest<TagDto>;
public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto>
{
    private readonly ITagRepository _repo;
    private readonly ICurrentUserService _user;
    private readonly IMapper _mapper;

    public GetTagByIdQueryHandler(ITagRepository repo, ICurrentUserService user, IMapper mapper)
    { _repo = repo; _user = user; _mapper = mapper; }

    public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _repo.GetByIdAsync(request.Id, _user.UserId, cancellationToken);
        if (tag == null) throw new Exception("Không tìm thấy thẻ.");
        return _mapper.Map<TagDto>(tag);
    }
}