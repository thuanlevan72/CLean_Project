namespace Domain.Enums;

/// <summary>
/// PriorityLevel Enum - Mức độ ưu tiên công việc
/// 
/// Mô tả:
/// - Định nghĩa các mức độ ưu tiên cho công việc (Todo)
/// - Dùng để sắp xếp, filter, và hiển thị công việc theo mức độ quan trọng
/// - Giá trị số (0-4) cho phép so sánh và sắp xếp dễ dàng
/// 
/// Cách sử dụng:
/// - Khi tạo/update todo: chọn mức độ ưu tiên
/// - API response: hiển thị với label và color tương ứng
/// - Sorting: sắp xếp todos theo Priority (descending)
/// 
/// Ví dụ sắp xếp:
/// var todos = dbContext.TodoItems
///     .Where(x => !x.IsDeleted)
///     .OrderByDescending(x => x.Priority)  // Ưu tiên cao trước
///     .ToList();
/// </summary>
public enum PriorityLevel
{
    /// <summary>Không có ưu tiên cụ thể - dùng làm giá trị mặc định</summary>
    None = 0,

    /// <summary>Ưu tiên thấp - công việc không cấp bách, có thể làm vào lúc rảnh</summary>
    Low = 1,

    /// <summary>Ưu tiên bình thường - công việc bình thường trong quy trình hàng ngày</summary>
    Medium = 2,

    /// <summary>Ưu tiên cao - công việc cấp bách, cần được hoàn thành sớm</summary>
    High = 3,

    /// <summary>Ưu tiên cực cao (khẩn cấp) - công việc ảnh hưởng kinh doanh, cần ngay lập tức</summary>
    Urgent = 4
}

/// <summary>
/// TodoStatus Enum - Trạng thái công việc
/// 
/// Mô tả:
/// - Định nghĩa các trạng thái cho vòng đời công việc
/// - Dùng để theo dõi tiến độ công việc từ tạo đến hoàn thành
/// - Workflow: Todo → InProgress → InReview → Completed (hoặc Cancelled)
/// 
/// Cách sử dụng:
/// - Khi tạo todo: Status = Todo
/// - Khi bắt đầu: Status = InProgress
/// - Khi hoàn thành: Status = Completed + set CompletedAt = now
/// - Khi hủy: Status = Cancelled (nếu cần)
/// 
/// Metrics:
/// - Tính completion rate: (Completed / Total) * 100
/// - Track velocity: Bao nhiêu todos completed per sprint
/// - Filter by status: Hiển thị "My Active Todos" = Status != Completed && Status != Cancelled
/// 
/// Database:
/// - Lưu trữ dưới dạng int (0-4) trong database
/// - EF Core tự động convert enum ↔ int
/// </summary>
public enum TodoStatus
{
    /// <summary>
    /// Todo - Công việc chưa bắt đầu
    /// 
    /// Mô tả: Công việc vừa được tạo, chưa có ai bắt tay vào làm
    /// Ưu tiên: Công việc trong backlog, chưa assigned cho sprint
    /// Chuyển tiếp: Todo → InProgress (khi bắt đầu làm)
    /// </summary>
    Todo = 0,

    /// <summary>
    /// InProgress - Công việc đang thực hiện
    /// 
    /// Mô tả: Công việc được assign cho ai đó, đang được làm
    /// Duration: Có thể kéo dài từ vài giờ đến vài ngày
    /// Chuyển tiếp: InProgress → InReview (khi done, chờ review) hoặc → Completed (nếu không cần review)
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// InReview - Công việc chờ review
    /// 
    /// Mô tả: Công việc đã hoàn thành, đang chờ review/approval từ manager/lead
    /// Ví dụ: Code đang chờ code review, document chờ approval
    /// Chuyển tiếp: InReview → Completed (nếu approve) hoặc → InProgress (nếu cần fix)
    /// </summary>
    InReview = 2,

    /// <summary>
    /// Completed - Công việc hoàn thành
    /// 
    /// Mô tả: Công việc đã done, approved, và closed
    /// CompletedAt: Được set khi Status chuyển sang Completed
    /// Metrics: Dùng để tính productivity, velocity
    /// Chuyển tiếp: Completed → ? (thường không chuyển, hoặc → Todo nếu reopen)
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Cancelled - Công việc bị hủy
    /// 
    /// Mô tả: Công việc không cần làm nữa, bị hủy (có lý do)
    /// Lý do phổ biến: Duplicate, Out of scope, Priority changed, etc.
    /// Chuyển tiếp: Cancelled → Todo (nếu uncancelled/reopen)
    /// Metrics: Exclude from velocity calculation
    /// </summary>
    Cancelled = 4
}