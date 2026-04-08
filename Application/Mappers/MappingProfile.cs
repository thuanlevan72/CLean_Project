using Application.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Tự động map từ Entity sang DTO
        // AutoMapper sẽ tự hiểu Category.Name map vào CategoryName nhờ naming convention
        CreateMap<TodoItem, TodoDto>()
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Category, CategoryDto>();
        CreateMap<Tag, TagDto>();
    }
}