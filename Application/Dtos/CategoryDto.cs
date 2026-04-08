namespace Application.Dtos;

/// <summary>
/// CategoryDto: Data Transfer Object cho Category
/// 
/// Mô tả:
/// - Lightweight version của Category entity
/// - Dùng để transfer category data giữa API ↔ Client
/// - Remove sensitive fields (UserId, IsDeleted, etc.)
/// - Simple flat structure
/// 
/// So sánh Category entity vs CategoryDto:
/// 
/// Category Entity:
/// - Id, Name, ColorHex
/// - UserId (who owns category)
/// - CreatedAt, UpdatedAt, DeletedAt (audit)
/// - IsDeleted (soft delete flag)
/// - TodoItems (navigation collection)
/// 
/// CategoryDto (exposed to client):
/// - Id, Name, ColorHex (all visible)
/// - No audit fields (internal only)
/// - No navigation properties (avoid circular refs, large payloads)
/// - No UserId (not relevant to client)
/// 
/// Cách tạo CategoryDto:
/// - AutoMapper: category → _mapper.Map<CategoryDto>(category)
/// - API Response: return Ok(categoryDto)
/// 
/// Client sử dụng:
/// - Display category name ở list
/// - Show color indicator ở UI
/// - Use Id để reference category khi create/update todos
/// </summary>
public record CategoryDto(
    /// <summary>
    /// Định danh duy nhất của category
    /// - Type: Guid
    /// - Dùng: Client use để reference khi assign todo to category
    /// - Ví dụ: "550e8400-e29b-41d4-a716-446655440000"
    /// </summary>
    Guid Id, 

    /// <summary>
    /// Tên danh mục
    /// - Type: string (required)
    /// - Hiển thị: ở dropdown, sidebar, category list
    /// - Ví dụ: "Work", "Personal", "Shopping"
    /// </summary>
    string Name, 

    /// <summary>
    /// Màu danh mục (Hex)
    /// - Type: string? (nullable, optional)
    /// - Format: #RRGGBB (e.g., #FF0000 = red)
    /// - Hiển thị: Category badge/tag với màu này
    /// - Frontend: Use to style category chips/badges
    /// - Ví dụ: "#FF0000", "#00FF00", "#0000FF"
    /// </summary>
    string? ColorHex
);