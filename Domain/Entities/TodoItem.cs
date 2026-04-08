using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// TodoItem: Entity đại diện cho một mục công việc (Todo)
/// 
/// Mô tả:
/// - Đây là aggregate root của bounded context "Todo Management"
/// - Mỗi TodoItem thuộc sở hữu của một User
/// - Có thể có Category để phân loại
/// - Có thể gán Tags để đánh dấu
/// - Hỗ trợ sub-tasks (công việc con)
/// - Hỗ trợ soft delete thông qua BaseEntity<Guid>
/// 
/// Relationships:
/// - One-to-One: User (UserId)
/// - Many-to-One: Category (CategoryId)
/// - Many-to-Many: Tags (ICollection)
/// - One-to-Many: SubTasks (hierarchy)
/// 
/// Validations (được thực hiện ở Application layer):
/// - Title không được rỗng, độ dài 1-200 ký tự
/// - Description tối đa 2000 ký tự
/// - DueDate phải >= ngày hôm nay
/// - Priority phải là một giá trị enum hợp lệ
/// - Status phải là một giá trị enum hợp lệ
/// </summary>
public class TodoItem : BaseEntity<Guid>
{
    /// <summary>
    /// Tiêu đề công việc
    /// 
    /// Mục đích:
    /// - Mô tả ngắn gọn công việc cần làm
    /// - Hiển thị ở danh sách, notification, email
    /// 
    /// Yêu cầu:
    /// - Bắt buộc, không được rỗng
    /// - Độ dài 1-200 ký tự
    /// - Ví dụ: "Hoàn thành báo cáo quarterly", "Fix bug login", etc.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả chi tiết công việc
    /// 
    /// Mục đích:
    /// - Cung cấp thông tin chi tiết, context về công việc
    /// - Giúp người thực hiện hiểu rõ hơn
    /// - Tùy chọn (nullable)
    /// 
    /// Ví dụ:
    /// - "Q4 report bao gồm: revenue, expenses, growth metrics"
    /// - "Bug: User không thể login với email chứa + ký tự"
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Mức độ ưu tiên công việc
    /// 
    /// Mục đích:
    /// - Xác định mức độ cấp bách
    /// - Ảnh hưởng đến thứ tự hiển thị, notification
    /// 
    /// Giá trị (enum PriorityLevel):
    /// - Low: Công việc không cấp bách
    /// - Medium: Công việc bình thường
    /// - High: Công việc cấp bách
    /// - Critical: Công việc rất cấp bách, ảnh hưởng kinh doanh
    /// 
    /// Cách sử dụng:
    /// - Hiển thị với icon/color khác nhau theo priority
    /// - Sắp xếp danh sách theo priority
    /// - Alert/notification nếu high priority item quá hạn
    /// </summary>
    public PriorityLevel Priority { get; set; }

    /// <summary>
    /// Trạng thái hiện tại của công việc
    /// 
    /// Mục đích:
    /// - Theo dõi tiến độ công việc
    /// - Phân biệt công việc chưa làm, đang làm, đã làm
    /// 
    /// Giá trị (enum TodoStatus):
    /// - Todo: Công việc chưa bắt đầu
    /// - InProgress: Công việc đang thực hiện
    /// - Done: Công việc đã hoàn thành
    /// - Blocked: Công việc bị chặn (đang chờ cái gì đó)
    /// - OnHold: Công việc tạm dừng (sẽ tiếp tục sau)
    /// 
    /// Cách thay đổi:
    /// - Gọi UpdateTodoStatusCommand từ API
    /// - Tự động set CompletedAt khi Status -> Done
    /// - Ghi lại thời gian chuyển đổi
    /// </summary>
    public TodoStatus Status { get; set; }

    /// <summary>
    /// Thời hạn công việc
    /// 
    /// Mục đích:
    /// - Xác định deadline cần hoàn thành công việc
    /// - Tính toán thời gian còn lại
    /// - Nhắc nhở nếu sắp hết hạn
    /// 
    /// Giá trị:
    /// - Nullable: Công việc có thể không có deadline
    /// - DateTimeOffset: Để phòng time zone issues
    /// 
    /// Cách sử dụng:
    /// - Filter công việc "quá hạn" (DueDate < now)
    /// - Sort công việc theo DueDate
    /// - Notification: "3 items due tomorrow"
    /// </summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Thời điểm hoàn thành công việc
    /// 
    /// Mục đích:
    /// - Ghi lại khi nào công việc được đánh dấu hoàn thành
    /// - Tính toán thời gian từ tạo đến hoàn thành
    /// 
    /// Giá trị:
    /// - Nullable: Công việc chưa hoàn thành → null
    /// - Có giá trị: Công việc đã hoàn thành (Status = Done)
    /// - DateTimeOffset: UTC timestamp
    /// 
    /// Logic:
    /// - Khi Status thay đổi sang Done → Set CompletedAt = DateTimeOffset.UtcNow
    /// - Khi Status thay đổi từ Done sang khác → Clear CompletedAt = null
    /// - Dùng để tính "velocity" (productivity metrics)
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Định danh người sở hữu công việc
    /// 
    /// Mục đích:
    /// - Liên kết công việc với User
    /// - Xác định ai là chủ công việc
    /// - Dùng để filter "My Todos"
    /// 
    /// Foreign Key:
    /// - Tham chiếu đến bảng Users
    /// - Bắt buộc, không null
    /// - Khi user bị xóa: Tùy thuộc cascade delete policy
    /// 
    /// Security:
    /// - Controllers phải kiểm tra UserId == currentUserId
    /// - Người dùng chỉ có thể xem/edit công việc của họ
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Định danh danh mục công việc (Optional)
    /// 
    /// Mục đích:
    /// - Gán công việc vào danh mục (category)
    /// - Phân loại công việc: Work, Personal, Shopping, etc.
    /// - Hiển thị công việc theo danh mục
    /// 
    /// Foreign Key:
    /// - Tham chiếu đến bảng Categories
    /// - Nullable: Công việc có thể không có category
    /// 
    /// Navigation Property:
    /// - Category: Load thông tin category nếu cần
    /// - Dùng EF Core eager/lazy loading
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Navigation property: Thông tin chi tiết danh mục
    /// 
    /// Mục đích:
    /// - Truy cập thông tin Category liên quan (name, description, color)
    /// - Cho phép eager loading khi query
    /// 
    /// Ví dụ:
    /// var todo = context.TodoItems
    ///     .Include(t => t.Category)  // Eager load category
    ///     .FirstOrDefault();
    /// // Có thể truy cập todo.Category.Name
    /// </summary>
    public Category? Category { get; set; }

    /// <summary>
    /// Định danh công việc cha (Optional)
    /// 
    /// Mục đích:
    /// - Hỗ trợ hierarchical todos (công việc lớn có công việc con)
    /// - Ví dụ: "Plan Q4" (cha) có sub-tasks "Plan revenue", "Plan expenses"
    /// - Cho phép group related todos
    /// 
    /// Giá trị:
    /// - Nullable: Công việc có thể không có công việc cha (root-level)
    /// - Tự tham chiếu (Self-referencing foreign key)
    /// 
    /// Cascade Delete:
    /// - Khi xóa công việc cha → xóa/orphan tất cả sub-tasks
    /// - Tùy thuộc cấu hình EF Core
    /// </summary>
    public Guid? ParentTaskId { get; set; }

    /// <summary>
    /// Navigation property: Công việc cha
    /// 
    /// Mục đích:
    /// - Truy cập thông tin công việc cha
    /// - Dùng để display breadcrumb hoặc parent info
    /// </summary>
    public TodoItem? ParentTask { get; set; }

    /// <summary>
    /// Danh sách công việc con (Sub-tasks)
    /// 
    /// Mục đích:
    /// - Quản lý công việc phụ thuộc vào công việc này
    /// - Ví dụ: "Plan Q4" có multiple sub-tasks
    /// - Tính tiến độ dựa trên hoàn thành sub-tasks
    /// 
    /// Collection:
    /// - Initialized = new List<TodoItem>() để tránh null
    /// - One-to-Many relationship (1 parent : many children)
    /// 
    /// Cách sử dụng:
    /// - Eager load: .Include(t => t.SubTasks)
    /// - Calculate progress: (completed sub-tasks / total) * 100
    /// - Recursive delete: Xóa cha → xóa con
    /// </summary>
    public ICollection<TodoItem> SubTasks { get; set; } = new List<TodoItem>();

    /// <summary>
    /// Danh sách tags gán cho công việc
    /// 
    /// Mục đích:
    /// - Tag công việc với labels tùy chọn
    /// - Ví dụ: tags = ["urgent", "client-facing", "backend"]
    /// - Cho phép filter/search theo tags
    /// 
    /// Many-to-Many Relationship:
    /// - Một TodoItem có nhiều Tags
    /// - Một Tag có nhiều TodoItems
    /// - Mapper bảng join được EF Core quản lý
    /// 
    /// Cách sử dụng:
    /// - Eager load: .Include(t => t.Tags)
    /// - Display tags với colors/icons khác nhau
    /// - Search: Find todos with tag "urgent"
    /// - Aggregate: Count todos per tag
    /// </summary>
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}