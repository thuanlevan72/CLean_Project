namespace Application.Interfaces;

/// <summary>
/// ICacheService: Service để quản lý distributed cache (Redis)
/// 
/// Mô tả:
/// - Abstraction layer cho Redis caching operations
/// - Được implement ở Infrastructure.Redis layer
/// - Dùng để cache dữ liệu thường xuyên được access
/// - Giảm database load, improve response time
/// 
/// Caching Layers (3-tier cache strategy):
/// 1. L1: In-Memory cache (MemoryCache - local process) - fast, limited size
/// 2. L2: Distributed cache (Redis) - shared across processes/servers, larger size
/// 3. L3: Database (PostgreSQL) - persistent, slowest
/// 
/// Cache Strategy:
/// - Cache-Aside Pattern (Lazy Loading):
///   1. Check cache first
///   2. If miss: Query database
///   3. Save to cache
///   4. Return data
/// - Time-based expiration: sliding + absolute TTL
/// - Pattern-based invalidation: Remove by prefix
/// 
/// Redis Client: StackExchange.Redis library
/// - Key-Value store: Simple string keys, JSON values
/// - TTL: Automatic expiration
/// - Pub/Sub: For real-time events
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Lấy dữ liệu từ cache
    /// 
    /// Mục đích:
    /// - Truy vấn dữ liệu đã lưu trong Redis
    /// - Fast retrieval (sub-millisecond latency)
    /// - Reduce database queries
    /// 
    /// Tham số:
    /// - key: Cache key (string identifier)
    /// - cancellationToken: Async cancellation support
    /// 
    /// Trả về:
    /// - T: Deserialized object nếu cache hit
    /// - Null: Nếu key không tồn tại hoặc expired
    /// 
    /// Generics:
    /// - T: Tự động convert JSON → object bằng JsonSerializer
    /// - Ví dụ: GetAsync<TodoDto>("todo_123")
    /// 
    /// Ví dụ:
    /// var cachedTodo = await _cache.GetAsync<TodoDto>("todo_550e8400");
    /// if (cachedTodo != null) 
    /// {
    ///     return cachedTodo;  // Cache hit
    /// }
    /// // Cache miss - query database below
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lưu dữ liệu vào cache
    /// 
    /// Mục đích:
    /// - Store objects trong Redis
    /// - Enable caching strategy
    /// - Set expiration time (TTL)
    /// 
    /// Tham số:
    /// - key: Cache key để identify data
    /// - value: Object cần cache
    /// - slidingExpiration: TTL nếu accessed (ví dụ: 5 min)
    ///   * Nếu access trước expiration → reset timer
    ///   * Ví dụ: User xem todo → reset timer → không expiration yet
    /// - absoluteExpiration: Luôn expiration sau thời gian này
    ///   * Không reset dù có access
    ///   * Ví dụ: Cache luôn expire sau 1 giờ, đảm bảo fresh data
    /// - cancellationToken: Async cancellation
    /// 
    /// Expiration Logic:
    /// - slidingExpiration = 5min, absoluteExpiration = null
    ///   → Expire nếu không access 5 phút
    /// - slidingExpiration = null, absoluteExpiration = 1hour
    ///   → Expire sau 1 giờ regardless of access
    /// - Both set: Whichever comes first
    /// - Both null: Never expire (not recommended)
    /// 
    /// Ví dụ:
    /// var todoDto = new TodoDto(...);
    /// await _cache.SetAsync<TodoDto>(
    ///     "todo_550e8400", 
    ///     todoDto,
    ///     slidingExpiration: TimeSpan.FromMinutes(5),
    ///     absoluteExpiration: TimeSpan.FromHours(1)
    /// );
    /// </summary>
    Task SetAsync<T>(
        string key, 
        T value, 
        TimeSpan? slidingExpiration = null, 
        TimeSpan? absoluteExpiration = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Xóa dữ liệu khỏi cache
    /// 
    /// Mục đức:
    /// - Invalidate cache entry
    /// - Force next request to query database (refresh)
    /// - Dùng khi dữ liệu thay đổi
    /// 
    /// Tham số:
    /// - key: Cache key cần delete
    /// - cancellationToken: Async cancellation
    /// 
    /// Ví dụ (Invalidate on Update):
    /// // Handler: Update todo
    /// var todo = await _todoRepository.GetByIdAsync(id);
    /// todo.Title = "New title";
    /// _todoRepository.Update(todo);
    /// await _unitOfWork.SaveAsync();
    /// 
    /// // Invalidate cache
    /// await _cache.RemoveAsync($"todo_{id}");
    /// 
    /// // Next request: Cache miss → query DB → get fresh data
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy từ cache hoặc set nếu miss (Cache-Aside Pattern)
    /// 
    /// Mục đích:
    /// - Implement cache-aside pattern cực kỳ gọn gàng
    /// - Kiểm tra cache, nếu miss thì query factory (database)
    /// - Lưu result vào cache, return
    /// 
    /// Tham số:
    /// - key: Cache key
    /// - factory: Function để query data (usually database)
    ///   * Type: Func<CancellationToken, Task<T>>
    ///   * Gọi khi cache miss
    ///   * Thường là: ct => _todoRepository.GetByIdAsync(id, ct)
    /// - slidingExpiration, absoluteExpiration: TTL options
    /// - cancellationToken: Async cancellation
    /// 
    /// Flow:
    /// 1. Cek cache với key
    /// 2. Nếu hit: Return ngay
    /// 3. Nếu miss: Gọi factory(cancellationToken)
    /// 4. Wait for result từ database
    /// 5. Lưu result vào cache
    /// 6. Return result
    /// 
    /// Ví dụ (Thực tế):
    /// var todo = await _cache.GetOrSetAsync<TodoDto>(
    ///     $"todo_{id}",
    ///     async ct => 
    ///     {
    ///         var todoItem = await _todoRepository.GetByIdAsync(id, ct);
    ///         return _mapper.Map<TodoDto>(todoItem);
    ///     },
    ///     slidingExpiration: TimeSpan.FromMinutes(5)
    /// );
    /// // Nếu cache hit: return từ Redis (< 1ms)
    /// // Nếu cache miss: query DB (50-100ms), save to cache, return
    /// 
    /// Benefit:
    /// - Cực kỳ concise code (1 line instead of 5-10)
    /// - Automatic cache management
    /// - Thread-safe implementation
    /// </summary>
    Task<T> GetOrSetAsync<T>(
        string key, 
        Func<CancellationToken, Task<T>> factory, 
        TimeSpan? slidingExpiration = null, 
        TimeSpan? absoluteExpiration = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Xóa danh sách keys theo pattern (Prefix)
    /// 
    /// Mục đích:
    /// - Bulk invalidate: Xóa nhiều cache keys cùng lúc
    /// - Hữu ích khi cần clear all user's data
    /// - Ví dụ: User update profile → clear all user's todos cache
    /// 
    /// Tham số:
    /// - prefixKey: Prefix pattern để match keys
    /// - cancellationToken: Async cancellation
    /// 
    /// Pattern Matching:
    /// - Redis SCAN command với pattern
    /// - "*" = wildcard
    /// - Ví dụ: "user_123_*" match tất cả keys start với "user_123_"
    /// 
    /// Ví dụ (Invalidate on User Change):
    /// // User update profile
    /// await _cache.RemoveByPrefixAsync($"user_{userId}_*");
    /// 
    /// // Sẽ delete:
    /// // - user_123_todos
    /// // - user_123_categories
    /// // - user_123_profile
    /// // - user_123_settings
    /// // - etc.
    /// 
    /// Performance:
    /// - SCAN: Non-blocking, iterative scan
    /// - Bulk delete: Delete multiple keys efficiently
    /// - Async: Non-blocking I/O
    /// </summary>
    Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
}