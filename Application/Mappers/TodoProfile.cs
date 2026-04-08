//using Application.Dtos;
//using AutoMapper;
//using TodoApi.Models;
//namespace Application.Mappers
//{
//    public class TodoProfile: Profile
//    {
//        public TodoProfile()
//        {
//            CreateMap<Todo, CreateTodoDto>();

//            // map ngược
//            CreateMap<CreateTodoDto, Todo>();


//            CreateMap<Todo, UpdateTodoDto>();
//            CreateMap<UpdateTodoDto, Todo>();
//            CreateMap<TodoDto, Todo>();

//            //// custom mapping
//            //CreateMap<Product, ProductDto>()
//            //    .ForMember(dest => dest.PriceText,
//            //               opt => opt.MapFrom(src => $"{src.Price} VND"));
//        }
//    }
//}
