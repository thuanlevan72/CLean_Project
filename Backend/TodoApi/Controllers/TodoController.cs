using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
  private readonly ITodoService _service;

  public TodoController(ITodoService service)
  {
    _service = service;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<TodoDto>>> GetAll()
  {
    var todos = await _service.GetAllTodosAsync();
    return Ok(todos);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TodoDto>> GetById(int id)
  {
    var todo = await _service.GetTodoByIdAsync(id);
    if (todo == null) return NotFound();
    return Ok(todo);
  }

  [HttpPost]
  public async Task<ActionResult<TodoDto>> Create(CreateTodoDto dto)
  {
    if (string.IsNullOrWhiteSpace(dto.Text))
      return BadRequest("Text is required");

    var todo = await _service.CreateTodoAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<TodoDto>> Update(int id, UpdateTodoDto dto)
  {
    var todo = await _service.UpdateTodoAsync(id, dto);
    if (todo == null) return NotFound();
    return Ok(todo);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    var result = await _service.DeleteTodoAsync(id);
    if (!result) return NotFound();
    return NoContent();
  }

  [HttpPatch("{id}/toggle")]
  public async Task<ActionResult<TodoDto>> Toggle(int id)
  {
    var todo = await _service.ToggleTodoAsync(id);
    if (todo == null) return NotFound();
    return Ok(todo);
  }
}
