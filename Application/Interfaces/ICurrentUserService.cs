namespace Application.Interfaces;

/// <summary>
/// ICurrentUserService: Service để lấy thông tin current user
/// 
/// Mô tả:
/// - Abstraction layer để lấy user info từ JWT token
/// - Được implement ở API layer (Api/Extensions/CurrentUserService.cs)
/// - Dùng ở Application layer handlers để lấy current user ID
/// - Giảm coupling: Domain/Application không phụ thuộc HttpContext
/// 
/// Flow:
/// 1. Middleware decode JWT token từ Authorization header
/// 2. Extract userId claim từ token
/// 3. Store vào HttpContext.Items hoặc thread-local storage
/// 4. Service retrieve từ HttpContext.Items
/// 5. Handler inject service, gọi UserId/IsAuthenticated
/// 
/// Implementation Strategy:
/// - API layer implement bằng HttpContextAccessor
/// - Scope: Scoped (new instance per request)
/// - Thread-safe: HttpContext scope-bound per request
/// 
/// Lợi ích:
/// - Security: Không cho user set UserId arbitrary
/// - Audit: Tự động record who created/modified data
/// - Authorization: Check if user can access resource
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Lấy định danh người dùng hiện tại
    /// 
    /// Mục đích:
    /// - Lấy UserId từ JWT token claims
    /// - Dùng để filter/authorize operations
    /// - Đảm bảo user chỉ access được data của họ
    /// 
    /// Kiểu dữ liệu:
    /// - Guid: Unique identifier của user (assigned during registration)
    /// 
    /// Ví dụ:
    /// var userId = _currentUserService.UserId;
    /// var userTodos = await _todoRepository.GetAllByUserIdAsync(userId);
    /// 
    /// Cách set:
    /// - JWT token payload chứa "sub" (subject) claim = userId
    /// - Token được decode ở authentication middleware
    /// - UserId được extract từ claims
    /// 
    /// Exception:
    /// - InvalidOperationException nếu không authenticated (user not logged in)
    /// - Luôn check IsAuthenticated trước khi access UserId
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Kiểm tra user hiện tại đã authenticated hay chưa
    /// 
    /// Mục đích:
    /// - Kiểm tra xem request có JWT token hợp lệ không
    /// - Dùng để guard operations (chỉ authenticated users)
    /// - Throw exception nếu user chưa login
    /// 
    /// Kiểu dữ liệu:
    /// - bool: true = authenticated, false = anonymous
    /// 
    /// Ví dụ:
    /// if (!_currentUserService.IsAuthenticated)
    ///     throw new UnauthorizedAccessException("Bạn chưa đăng nhập");
    /// 
    /// var userId = _currentUserService.UserId;  // Safe to call
    /// 
    /// Cách set:
    /// - true: JWT token valid, user claims có "sub"
    /// - false: No token hoặc token invalid
    /// 
    /// Common checks:
    /// - Start of handler: Verify IsAuthenticated
    /// - Then: Safe to use UserId
    /// - Never: Assume authenticated without checking
    /// </summary>
    bool IsAuthenticated { get; }
}