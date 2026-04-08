
using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// Tag: Entity đại diện cho một nhãn/tag
/// 
/// Mô tả:
/// - Dùng để gán nhãn/label cho Todos
/// - Many-to-Many relationship: 1 tag có nhiều todos, 1 todo có nhiều tags
/// - Khác Category: 1 todo có 1 category, nhưng có thể có multiple tags
/// - Ví dụ tags: "urgent", "client-facing", "backend", "frontend", etc.
/// 
/// Relationships:
/// - User owns multiple Tags (One-to-Many via UserId)
/// - Todos have multiple Tags (Many-to-Many via join table)
/// 
/// Validations:
/// - Name: Required, 1-50 characters, unique per user
/// - UserId: Required, must match current user
/// 
/// Primary Key Type:
/// - Note: Tag sử dụng `int` thay vì `Guid` (khác với Category)
/// - Lý do: Tags thường nhỏ, ít thay đổi, int PK đủ dùng
/// - Performance: int nhỏ hơn Guid, query nhanh hơn
/// 
/// Ví dụ sử dụng:
/// // Tạo tags
/// var tags = new[] { "urgent", "backend" }
///     .Select(name => new Tag { Name = name, UserId = userId })
///     .ToList();
/// 
/// // Assign tags to todo
/// var todo = new TodoItem { Title = "..." };
/// todo.Tags.Add(tags[0]);
/// todo.Tags.Add(tags[1]);
/// </summary>
public class Tag : BaseEntity<int>
{
    /// <summary>
    /// Tên tag
    /// 
    /// Mục đích:
    /// - Mô tả ý nghĩa của tag
    /// - Hiển thị ở UI như labels/badges
    /// - Dùng để filter/search todos
    /// 
    /// Yêu cầu:
    /// - Bắt buộc, không được rỗng
    /// - Độ dài 1-50 ký tự
    /// - Unique per user (constraint ở DB)
    /// 
    /// Ví dụ:
    /// - "urgent" - công việc cấp bách
    /// - "client-facing" - công việc liên quan khách hàng
    /// - "backend" - công việc backend
    /// - "frontend" - công việc frontend
    /// - "documentation" - công việc write docs
    /// - "bug-fix" - công việc fix bugs
    /// - "feature" - công việc feature mới
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Định danh người sở hữu tag
    /// 
    /// Mục đích:
    /// - Liên kết tag với User
    /// - Xác định ai tạo/sở hữu tag
    /// - Dùng để filter tags cho current user
    /// 
    /// Foreign Key:
    /// - Tham chiếu đến bảng Users
    /// - Bắt buộc, không null
    /// - Cascade delete: Khi user xóa → xóa tags
    /// 
    /// Security:
    /// - Controllers phải kiểm tra UserId == currentUserId
    /// - Không cho user khác access tag này
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property: Danh sách Todos với tag này
    /// 
    /// Mục đích:
    /// - Truy cập tất cả todos có tag này
    /// - Load all todos khi cần (via Include)
    /// 
    /// Relationship:
    /// - Many-to-Many: 1 tag có nhiều todos, 1 todo có nhiều tags
    /// - EF Core sẽ tạo join table tự động (TodoItemTags)
    /// - Initialized = new List<TodoItem>() để tránh null
    /// 
    /// Ví dụ:
    /// var urgentTag = await dbContext.Tags
    ///     .Where(t => t.Name == "urgent")
    ///     .Include(t => t.TodoItems)  // Eager load todos with this tag
    ///     .FirstOrDefaultAsync();
    /// 
    /// Console.WriteLine($"Total urgent todos: {urgentTag.TodoItems.Count}");
    /// 
    /// foreach (var todo in urgentTag.TodoItems)
    /// {
    ///     Console.WriteLine($"- {todo.Title}");
    /// }
    /// </summary>
    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}