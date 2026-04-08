using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Categories.Queries;

public record GetMyCategoriesQuery() : IRequest<List<CategoryDto>>;
public class GetMyCategoriesQueryHandler : IRequestHandler<GetMyCategoriesQuery, List<CategoryDto>>
{
    private readonly ICategoryRepository _repo;
    private readonly ICurrentUserService _user;
    private readonly IMapper _mapper;

    public GetMyCategoriesQueryHandler(ICategoryRepository repo, ICurrentUserService user, IMapper mapper)
    { _repo = repo; _user = user; _mapper = mapper; }

    public async Task<List<CategoryDto>> Handle(GetMyCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _repo.GetAllByUserIdAsync(_user.UserId, cancellationToken);
        return _mapper.Map<List<CategoryDto>>(categories);
    }
}

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _repo;
    private readonly ICurrentUserService _user;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(ICategoryRepository repo, ICurrentUserService user, IMapper mapper)
    { _repo = repo; _user = user; _mapper = mapper; }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _repo.GetByIdAsync(request.Id, _user.UserId, cancellationToken);
        if (category == null) throw new Exception("Không tìm thấy danh mục.");
        return _mapper.Map<CategoryDto>(category);
    }
}