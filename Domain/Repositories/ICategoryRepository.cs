using Domain.Entities;

namespace Domain.Repositories;

/// <summary>
/// ICategoryRepository: Specialized repository for Category entity
/// 
/// Mô tả:
/// - Extend IGenericRepository<Category, Guid>
/// - Add domain-specific queries for categories
/// - Enforce security: Always verify UserId
/// - Hide implementation complexity (EF Core details)
/// 
/// Security pattern:
/// - All queries filter by UserId
/// - Prevent user A accessing user B's categories
/// - Return null if unauthorized (vs throw exception)
/// 
/// Implementation:
/// - Infrastructure.Postgres/Repositories/CategoryRepository.cs
/// - Uses EF Core DbContext
/// 
/// Ví dụ flow:
/// 1. CreateTodoCommand check category: GetByIdAsync(categoryId, userId)
/// 2. If null: Throw exception "Category not found"
/// 3. If found: Verify CategoryId belongs to user (implicit in query)
/// 4. Safe to use category
/// </summary>
public interface ICategoryRepository : IGenericRepository<Category, Guid>
{
    /// <summary>
    /// Lấy category theo ID + UserId (Security-checked)
    /// 
    /// Mục đích:
    /// - Get single category with security verification
    /// - Ensure category belongs to current user
    /// - Prevent cross-user access
    /// 
    /// Tham số:
    /// - id: Category ID (Guid)
    /// - userId: Current user ID (for authorization)
    /// - cancellationToken: Async cancellation
    /// 
    /// Trả về:
    /// - Category nếu found AND belongs to user
    /// - Null nếu not found hoặc different user's category
    /// 
    /// SQL (pseudo):
    /// SELECT * FROM Categories 
    /// WHERE Id = @id 
    ///   AND UserId = @userId 
    ///   AND IsDeleted = false
    /// 
    /// Ví dụ (CreateTodo flow):
    /// var category = await _categoryRepository.GetByIdAsync(
    ///     categoryId,  // User specified category
    ///     currentUserId  // Current user
    /// );
    /// if (category == null)
    ///     throw new Exception("Category not found");
    /// // Now safe to use category - already verified it's current user's
    /// 
    /// Note:
    /// - Override base GetByIdAsync: Add userId parameter
    /// - More secure than generic GetByIdAsync(id) without user check
    /// - Database-enforced filtering (WHERE clause)
    /// </summary>
    Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy tất cả categories của user
    /// 
    /// Mục đích:
    /// - Query all user's categories
    /// - Filter by UserId
    /// - Exclude soft-deleted
    /// - Used để populate category dropdown ở create todo form
    /// 
    /// Tham số:
    /// - userId: User ID để filter
    /// - cancellationToken: Async cancellation
    /// 
    /// Trả về:
    /// - List<Category>: All active categories for user
    /// - Empty list nếu user chưa tạo categories
    /// 
    /// SQL (pseudo):
    /// SELECT * FROM Categories 
    /// WHERE UserId = @userId 
    ///   AND IsDeleted = false
    /// ORDER BY Name ASC
    /// 
    /// Ví dụ:
    /// var categories = await _categoryRepository.GetAllByUserIdAsync(userId);
    /// 
    /// // Response to client:
    /// // [
    /// //   { id: "...", name: "Work", colorHex: "#FF0000" },
    /// //   { id: "...", name: "Personal", colorHex: "#00FF00" }
    /// // ]
    /// 
    /// UI usage:
    /// - Display ở dropdown khi create todo
    /// - Display ở sidebar categories list
    /// - Filter todos by category
    /// 
    /// Performance:
    /// - Usually small dataset (users have 5-20 categories)
    /// - No pagination needed
    /// - Can cache result (invalidate on category change)
    /// 
    /// Ordering:
    /// - ORDER BY Name: Alphabetical for easy selection
    /// - Can add secondary: ORDER BY Name, CreatedAt DESC
    /// </summary>
    Task<List<Category>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}