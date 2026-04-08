namespace Application.Dtos;

/// <summary>
/// TagDto: Data Transfer Object cho Tag
/// 
/// Mô tả:
/// - Lightweight version của Tag entity
/// - Transfer tag data giữa API ↔ Client
/// - Remove sensitive/internal fields (UserId, TodoItems, etc.)
/// - Simple structure cho easy serialization
/// 
/// So sánh Tag entity vs TagDto:
/// 
/// Tag Entity:
/// - Id (int), Name, UserId
/// - CreatedAt, UpdatedAt, DeletedAt (audit)
/// - IsDeleted (soft delete)
/// - TodoItems (many-to-many navigation)
/// 
/// TagDto (exposed to client):
/// - Id (int), Name (all visible)
/// - No audit fields (internal)
/// - No navigation (avoid cycles, save bandwidth)
/// - No UserId (not relevant)
/// 
/// Cách tạo TagDto:
/// - AutoMapper: tag → _mapper.Map<TagDto>(tag)
/// - API Response: return Ok(tagDto) hoặc Ok(tagDtos)
/// 
/// Client sử dụng:
/// - Display tag names ở todos
/// - Show tag chips/badges
/// - Use Id để reference tags khi assign to todos
/// 
/// Note về Id type:
/// - Tag sử dụng int (khác Category dùng Guid)
/// - Lý do: Tags ít thay đổi, int PK đủ dùng
/// - Performance: int nhỏ, query nhanh hơn Guid
/// </summary>
public record TagDto(
    /// <summary>
    /// Định danh duy nhất của tag
    /// - Type: int (khác Category dùng Guid)
    /// - Range: 1 to 2,147,483,647 (int max value)
    /// - Dùng: Client use để reference tag
    /// - Ví dụ: 1, 42, 999
    /// </summary>
    int Id, 

    /// <summary>
    /// Tên tag
    /// - Type: string (required)
    /// - Hiển thị: Tag badge/chip ở todo detail
    /// - Ví dụ: "urgent", "client-facing", "backend", "frontend", "documentation"
    /// 
    /// Cách sử dụng ở client:
    /// - Filter todos by tag: "Show all urgent todos"
    /// - Color-code tags: urgent = red, backend = blue, etc.
    /// - Multi-select filter: Choose multiple tags
    /// </summary>
    string Name
);