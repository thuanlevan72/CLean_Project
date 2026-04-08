namespace Application.Dtos;

public record TodoDto(
    Guid Id,
    string Title,
    string? Description,
    string Priority,
    string Status,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    string? CategoryName
);