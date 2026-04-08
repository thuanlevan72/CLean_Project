//using Application.Dtos;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Application.Interfaces
//{
//    public interface ITodoCommandService
//    {
       
//        Task<TodoDto> CreateTodoAsync(CreateTodoDto dto);
//        Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto dto);
//        Task<bool> DeleteTodoAsync(int id);
//        Task<TodoDto?> ToggleTodoAsync(int id);
//    }


//    public interface ITodoQueryService
//    {
//        Task<IEnumerable<TodoDto>> GetAllTodosAsync();
//        Task<TodoDto?> GetTodoByIdAsync(int id);
//    }
//}
