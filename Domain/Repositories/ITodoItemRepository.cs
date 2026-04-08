using Domain.Entities;

namespace Domain.Repositories;

/// <summary>
/// ITodoItemRepository: Specialized repository for TodoItem entity
/// 
/// Mô tả:
/// - Extend IGenericRepository<TodoItem, Guid> để inherit CRUD operations
/// - Add domain-specific query methods
/// - Encapsulate complex query logic
/// - Enforce security (filter by UserId automatically)
/// 
/// Inherit from generic:
/// - GetByIdAsync, GetAllAsync: From IGenericRepository
/// - Add, Update, Delete: From IGenericRepository
/// 
/// Add specialized methods:
/// - GetByIdAndUserIdAsync: Get todo by ID + UserId (security)
/// - GetByIdWithDetailsAsync: Load todo + related data (eager load)
/// - GetAllByUserIdAsync: Get all user's todos
/// 
/// Security pattern:
/// - Always check UserId in queries
/// - Prevent user A accessing user B's todos
/// - Return null if UserId mismatch (instead of throw exception)
/// 
/// Implementation location:
/// - Infrastructure.Postgres/Repositories/TodoItemRepository.cs
/// - Uses EF Core DbContext to execute queries
/// </summary>
public interface ITodoItemRepository : IGenericRepository<TodoItem, Guid>
{
    /// <summary>
    /// Lấy todo theo ID + UserId (Security-checked get)
    /// 
    /// Mục đích:
    /// - Query single todo dengan security check
    /// - Verify todo belongs to current user
    /// - Prevent unauthorized access
    /// 
    /// Tham số:
    /// - id: Todo ID (Guid)
    /// - userId: Current user ID (for security verification)
    /// - cancellationToken: Async cancellation
    /// 
    /// Trả về:
    /// - TodoItem nếu found AND belongs to user
    /// - Null nếu not found hoặc belongs to different user
    /// 
    /// SQL (pseudo):
    /// SELECT * FROM TodoItems 
    /// WHERE Id = @id 
    ///   AND UserId = @userId 
    ///   AND IsDeleted = false
    /// 
    /// Ví dụ:
    /// var todo = await _repository.GetByIdAndUserIdAsync(
    ///     Guid.Parse("550e8400..."), 
    ///     currentUserId
    /// );
    /// if (todo == null) throw new NotFoundException("Todo not found");
    /// // Safe to use todo now - verified it belongs to current user
    /// 
    /// Security benefit:
    /// - One-liner security check
    /// - No need manual verification after query
    /// - Database-enforced (WHERE clause)
    /// </summary>
    Task<TodoItem?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy todo với tất cả related data (Eager loading)
    /// 
    /// Mục đích:
    /// - Get todo + related entities (Category, Tags, SubTasks, ParentTask)
    /// - Avoid N+1 query problem
    /// - Single query load all needed data
    /// - Include security check (UserId)
    /// 
    /// Tham số:
    /// - id: Todo ID (Guid)
    /// - userId: Current user ID (security)
    /// - cancellationToken: Async cancellation
    /// 
    /// Trả về:
    /// - TodoItem fully loaded với:
    ///   + Category (navigation property populated)
    ///   + Tags (collection loaded)
    ///   + SubTasks (child todos loaded)
    ///   + ParentTask (parent todo loaded)
    /// - Null nếu not found hoặc unauthorized
    /// 
    /// SQL (pseudo):
    /// SELECT t.*, c.*, tg.*, st.*, pt.*
    /// FROM TodoItems t
    /// LEFT JOIN Categories c ON t.CategoryId = c.Id
    /// LEFT JOIN TodoItemTags map ON t.Id = map.TodoItemId
    /// LEFT JOIN Tags tg ON map.TagId = tg.Id
    /// LEFT JOIN TodoItems st ON st.ParentTaskId = t.Id
    /// LEFT JOIN TodoItems pt ON t.ParentTaskId = pt.Id
    /// WHERE t.Id = @id AND t.UserId = @userId AND t.IsDeleted = false
    /// 
    /// Ví dụ sử dụng:
    /// var todo = await _repository.GetByIdWithDetailsAsync(id, userId);
    /// 
    /// // Ngay sau khi return từ query:
    /// // todo.Category.Name accessible (no lazy loading)
    /// // todo.Tags.Count accessible (collection loaded)
    /// // todo.SubTasks accessible (child todos loaded)
    /// // todo.ParentTask.Title accessible (parent loaded)
    /// 
    /// N+1 Problem Prevention:
    /// - Without eager loading:
    ///   1. Query TodoItem: 1 query
    ///   2. Access Category: +1 query
    ///   3. Access each Tag: +N queries
    ///   4. Access SubTasks: +1 query
    ///   Total: N+4 queries (slow!)
    /// 
    /// - With eager loading (.Include statements):
    ///   Single query loads all related data
    ///   Much faster for detail views
    /// </summary>
    Task<TodoItem?> GetByIdWithDetailsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy tất cả todos của một user
    /// 
    /// Mục đích:
    /// - Query tất cả todos belonging to specific user
    /// - Filter by UserId (security)
    /// - Exclude soft-deleted todos
    /// - Used ở "GetMyTodos" query
    /// 
    /// Tham số:
    /// - userId: User ID để filter todos
    /// - cancellationToken: Async cancellation
    /// 
    /// Trả về:
    /// - List<TodoItem>: All active todos for this user
    /// - Empty list nếu user không có todos
    /// 
    /// SQL (pseudo):
    /// SELECT * FROM TodoItems 
    /// WHERE UserId = @userId 
    ///   AND IsDeleted = false
    /// ORDER BY CreatedAt DESC
    /// 
    /// Ví dụ:
    /// var myTodos = await _repository.GetAllByUserIdAsync(currentUserId);
    /// var activeTodos = myTodos.Where(t => t.Status != TodoStatus.Completed).ToList();
    /// 
    /// Filtering & Sorting:
    /// - Database: Filter by UserId, IsDeleted (required for security/business)
    /// - Application: Sorting, additional filtering (by status, priority, etc.)
    /// - Why: Keep database query simple, filtering in memory (small dataset)
    /// 
    /// Performance consideration:
    /// - If user has thousands of todos: May need pagination
    /// - Future enhancement: Add GetAllByUserIdAsync(userId, skip, take) overload
    /// - Or add filtering parameters: GetAllByUserIdAsync(userId, status, priority)
    /// 
    /// N+1 query:
    /// - This query load todos only
    /// - Not loading Category/Tags (would need separate Include for each)
    /// - If need details: Use GetByIdWithDetailsAsync instead
    /// </summary>
    Task<List<TodoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}