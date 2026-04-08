using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// Category: Entity đại diện cho một danh mục
/// 
/// Mô tả:
/// - Dùng để phân loại/organize Todos
/// - Mỗi category thuộc sở hữu của một User
/// - Ví dụ categories: "Work", "Personal", "Shopping", "Health", etc.
/// - Mỗi Todo có thể gán vào 0 hoặc 1 category (Many-to-One)
/// 
/// Relationships:
/// - User owns multiple Categories (One-to-Many via UserId)
/// - Category contains multiple TodoItems (One-to-Many via ICollection)
/// 
/// Validations:
/// - Name: Required, 1-100 characters, unique per user
/// - ColorHex: Optional, valid hex color format (e.g., #FF0000)
/// - UserId: Required, must match current user
/// 
/// Ví dụ sử dụng:
/// // Tạo category
/// var category = new Category 
/// { 
///     Name = "Work", 
///     ColorHex = "#FF0000", 
///     UserId = userId 
/// };
/// 
/// // Assign todo to category
/// var todo = new TodoItem { Title = "...", CategoryId = category.Id };
/// </summary>
public class Category : BaseEntity<Guid>
{
    /// <summary>
    /// Tên danh mục
    /// 
    /// Mục đích:
    /// - Mô tả tên danh mục để người dùng nhận diện
    /// - Hiển thị ở sidebar, filter dropdown, category list
    /// 
    /// Yêu cầu:
    /// - Bắt buộc, không được rỗng
    /// - Độ dài 1-100 ký tự
    /// - Unique per user (2 users có thể có category cùng tên, nhưng 1 user không)
    /// 
    /// Ví dụ:
    /// - "Work", "Personal", "Shopping", "Health", "Learning", etc.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Màu danh mục (Hex color)
    /// 
    /// Mục đích:
    /// - Gán màu để phân biệt category visually
    /// - Hiển thị category với màu đó ở UI
    /// - Giúp user nhận diện nhanh
    /// 
    /// Format:
    /// - Hex color: #RRGGBB (e.g., #FF0000 = red, #00FF00 = green)
    /// - Optional: có thể null (use default color)
    /// 
    /// Ví dụ:
    /// - "#FF0000" = Red (for urgent/work tasks)
    /// - "#00FF00" = Green (for completed/personal)
    /// - "#0000FF" = Blue (for learning/hobbies)
    /// 
    /// Validation:
    /// - Regex: ^#[0-9A-F]{6}$ (valid hex format)
    /// </summary>
    public string? ColorHex { get; set; }

    /// <summary>
    /// Định danh người sở hữu danh mục
    /// 
    /// Mục đích:
    /// - Liên kết category với User
    /// - Xác định ai tạo/sở hữu category
    /// - Dùng để filter categories cho current user
    /// 
    /// Foreign Key:
    /// - Tham chiếu đến bảng Users
    /// - Bắt buộc, không null
    /// - Cascade delete: Khi user xóa → xóa categories
    /// 
    /// Security:
    /// - Controllers phải kiểm tra UserId == currentUserId
    /// - Không cho user khác access category này
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property: Danh sách Todos trong category này
    /// 
    /// Mục đích:
    /// - Truy cập tất cả todos gán vào category
    /// - Load all todos khi cần
    /// 
    /// Relationship:
    /// - One-to-Many: 1 category có nhiều todos
    /// - Initialized = new List<TodoItem>() để tránh null
    /// 
    /// Ví dụ:
    /// var category = await dbContext.Categories
    ///     .Include(c => c.TodoItems)  // Eager load todos
    ///     .FirstOrDefaultAsync();
    /// 
    /// foreach (var todo in category.TodoItems)
    /// {
    ///     Console.WriteLine(todo.Title);
    /// }
    /// </summary>
    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}