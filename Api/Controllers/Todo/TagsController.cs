using Application.Features.Tags.Commands;
using Application.Features.Tags.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetMyTagsQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetTagByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagCommand command)
    {
        var newId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { Message = "Tạo thẻ thành công", Id = newId });
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateTagCommand command)
    {
        var ids = await _mediator.Send(command);
        return Ok(new { Message = $"Đã tạo thành công {ids.Count} thẻ", Ids = ids });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTagRequest request)
    {
        await _mediator.Send(new UpdateTagCommand(id, request.Name));
        return Ok(new { Message = "Cập nhật thẻ thành công" });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteTagCommand(id));
        return Ok(new { Message = "Xóa thẻ thành công" });
    }
}

public record UpdateTagRequest(string Name);