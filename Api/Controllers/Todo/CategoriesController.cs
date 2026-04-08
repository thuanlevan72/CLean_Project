using Application.Features.Categories.Commands;
using Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetMyCategoriesQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var newId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { Message = "Tạo danh mục thành công", Id = newId });
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateCategoryCommand command)
    {
        var ids = await _mediator.Send(command);
        return Ok(new { Message = $"Đã tạo thành công {ids.Count} danh mục", Ids = ids });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        await _mediator.Send(new UpdateCategoryCommand(id, request.Name, request.ColorHex));
        return Ok(new { Message = "Cập nhật danh mục thành công" });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));
        return Ok(new { Message = "Xóa danh mục thành công" });
    }
}

// Record phụ để map Body cho API Update
public record UpdateCategoryRequest(string Name, string? ColorHex);