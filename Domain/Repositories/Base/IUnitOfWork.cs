namespace Domain.Repositories;

/// <summary>
/// IUnitOfWork: Unit of Work Pattern Interface
/// 
/// Mô tả:
/// - Quản lý transaction và commit tất cả database changes
/// - Đảm bảo atomicity (hoặc tất cả thay đổi, hoặc không thay đổi nào)
/// - Dùng trong CQRS handlers để coordinate repository operations
/// 
/// Pattern:
/// - Unit of Work: Logical transaction unit
/// - Repository Pattern + UnitOfWork = Complete data access abstraction
/// - Thay vì auto-commit, chúng ta manual commit một lần ở cuối
/// 
/// Workflow:
/// 1. Handler gọi repository.Add/Update/Delete multiple times
/// 2. Changes được track nhưng không commit
/// 3. Handler gọi unitOfWork.SaveChangesAsync()
/// 4. Tất cả changes commit cùng lúc (one transaction)
/// 5. Nếu có error → exception, autorollback
/// 
/// Cách sử dụng:
/// - Inject vào handler: constructor(IUnitOfWork unitOfWork)
/// - Gọi repository operations
/// - Gọi await unitOfWork.SaveChangesAsync() ở cuối
/// 
/// Lợi ích:
/// - Consistency: Tất cả hoặc không gì cả (ACID property)
/// - Error handling: Nếu 1 operation fail, tất cả rollback
/// - Performance: 1 round trip to DB instead of multiple
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Lưu tất cả changes vào database
    /// 
    /// Mục đích:
    /// - Commit tất cả changes (Add/Update/Delete) được tracked
    /// - Execute SQL statements cho tất cả pending changes
    /// - Gọi này một lần ở cuối handler
    /// 
    /// Tham số:
    /// - cancellationToken: Cho phép hủy operation
    /// 
    /// Trả về:
    /// - int: Số lượng entities bị ảnh hưởng (rows affected)
    /// - Ví dụ: 3 = 3 records inserted/updated/deleted
    /// 
    /// Exception:
    /// - DbUpdateException: Database constraint violation
    /// - DbUpdateConcurrencyException: Concurrency conflict
    /// - Các exceptions khác từ database
    /// 
    /// Ví dụ:
    /// var todo = new TodoItem { Title = "New" };
    /// repository.Add(todo);
    /// int affected = await unitOfWork.SaveChangesAsync();
    /// Console.WriteLine($"{affected} items inserted");
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Bắt đầu database transaction
    /// 
    /// Mục đích:
    /// - Mở transaction (explicit) cho manual transaction control
    /// - Thường dùng khi cần rollback nếu error
    /// - Optional: có thể bỏ qua nếu EF Core tự handle transactions
    /// 
    /// Cơ chế:
    /// - Database sẽ bắt đầu transaction
    /// - Tất cả subsequent SQL statements trong transaction này
    /// - Isolation level: Tùy thuộc database configuration
    /// 
    /// Ví dụ (Advanced):
    /// try
    /// {
    ///     await unitOfWork.BeginTransactionAsync();
    ///     repository.Add(todo1);
    ///     repository.Add(todo2);
    ///     await unitOfWork.SaveChangesAsync();
    ///     await unitOfWork.CommitTransactionAsync();
    /// }
    /// catch
    /// {
    ///     await unitOfWork.RollbackTransactionAsync();
    ///     throw;
    /// }
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit database transaction
    /// 
    /// Mục đích:
    /// - Confirm transaction, commit tất cả changes vào database
    /// - Phải gọi sau BeginTransactionAsync
    /// - Gọi sau SaveChangesAsync để finalize transaction
    /// 
    /// Cơ chế:
    /// - Database COMMIT command execute
    /// - Tất cả changes lưu vào database
    /// - Other connections có thể thấy changes
    /// 
    /// Ví dụ:
    /// await unitOfWork.BeginTransactionAsync();
    /// // ... do work ...
    /// await unitOfWork.SaveChangesAsync();
    /// await unitOfWork.CommitTransactionAsync();  // Tất cả changes lưu
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback database transaction
    /// 
    /// Mục đích:
    /// - Undo tất cả changes trong transaction
    /// - Gọi khi exception hoặc validation fail
    /// - Giúp maintain data consistency
    /// 
    /// Cơ chế:
    /// - Database ROLLBACK command execute
    /// - Tất cả changes revert
    /// - Database state quay lại trước transaction bắt đầu
    /// 
    /// Ví dụ (Error handling):
    /// try
    /// {
    ///     await unitOfWork.BeginTransactionAsync();
    ///     repository.Add(todo1);
    ///     repository.Add(todo2);
    ///     
    ///     if (todo2.Amount > MaxAmount) 
    ///         throw new ValidationException("Amount too large");
    ///     
    ///     await unitOfWork.SaveChangesAsync();
    ///     await unitOfWork.CommitTransactionAsync();
    /// }
    /// catch (Exception ex)
    /// {
    ///     await unitOfWork.RollbackTransactionAsync();  // Revert all changes
    ///     logger.LogError(ex);
    ///     throw;
    /// }
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}