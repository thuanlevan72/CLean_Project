using Domain.Entities.Base;

namespace Domain.Entities;

/// <summary>
/// OutboxMessage: Entity để store domain events trước khi publish
/// 
/// Mô tả:
/// - Implement Outbox Pattern (event sourcing)
/// - Store events trong database cùng transaction với data changes
/// - Đảm bảo consistency: Either data + event, or nothing
/// - Background job thường xuyên poll table này, publish to Kafka
/// - Nếu publish fail: Retry logic, không bao giờ mất event
/// 
/// Problem nó giải quyết:
/// - Dual-write problem: Write to DB + Kafka cùng lúc
/// - Nếu write DB success nhưng Kafka fail → inconsistent
/// - Solution: Write event + data to DB in same transaction
/// - Then: Separate process publish events from DB to Kafka
/// 
/// Flow:
/// 1. Handler tạo event: new TodoCreatedEvent { ... }
/// 2. Add event to outbox: outbox.Add(message)
/// 3. Save domain entity + outbox in same transaction
/// 4. Transaction commit hoặc rollback (atomic)
/// 5. Background job: Scan unprocessed messages
/// 6. Publish to Kafka: _kafkaPublisher.PublishAsync(event)
/// 7. Update ProcessedOnUtc: Đánh dấu đã gửi thành công
/// 
/// Benefits:
/// - No message loss: Events stored in DB
/// - Exactly-once delivery: Idempotent key prevents duplicates
/// - Audit trail: History of all events
/// - Replayability: Can replay events from database
/// 
/// Databases table:
/// - OutboxMessages table stores messages
/// - Indexed by CreatedOnUtc for efficient polling
/// - Indexed by ProcessedOnUtc for quick find unprocessed
/// </summary>
public class OutboxMessage: BaseEntity<Guid>
{
    /// <summary>
    /// Kafka topic name
    /// 
    /// Mục đích:
    /// - Định danh topic để publish message
    /// - Kafka consumers subscribe vào topic này
    /// 
    /// Ví dụ:
    /// - "todo.created" - Event khi todo được tạo
    /// - "todo.updated" - Event khi todo được update
    /// - "todo.deleted" - Event khi todo được xóa
    /// - "todo.completed" - Event khi todo completed
    /// 
    /// Naming convention:
    /// - Format: "aggregate.event"
    /// - Lowercase, dot-separated
    /// - Ví dụ: "category.created", "user.registered"
    /// </summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// JSON payload của event
    /// 
    /// Mục đích:
    /// - Chứa dữ liệu event serialize thành JSON string
    /// - Khi publish to Kafka, send payload này
    /// - Consumers deserialize JSON để lấy event data
    /// 
    /// Cấu trúc (Example):
    /// ```json
    /// {
    ///   "eventId": "550e8400-e29b-41d4-a716-446655440000",
    ///   "aggregateId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    ///   "aggregateType": "TodoItem",
    ///   "eventType": "TodoCreatedEvent",
    ///   "timestamp": "2024-12-20T10:30:00Z",
    ///   "data": {
    ///     "title": "Buy milk",
    ///     "priority": "High",
    ///     "dueDate": "2024-12-21T17:00:00Z"
    ///   }
    /// }
    /// ```
    /// 
    /// Lưu ý:
    /// - Already serialized to JSON (as string)
    /// - Text type in database (MAX length)
    /// - Compression có thể áp dụng nếu cần
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Thời điểm tạo message (UTC)
    /// 
    /// Mục đích:
    /// - Record khi event được tạo
    /// - Dùng để sort/filter by creation time
    /// - Audit trail: Biết event tạo lúc nào
    /// 
    /// Kiểu dữ liệu:
    /// - DateTime: UTC format (always UtcNow, không local time)
    /// - Default: DateTime.UtcNow (auto set)
    /// 
    /// Ví dụ:
    /// - "2024-12-20T10:30:45.123Z"
    /// 
    /// Indexing:
    /// - Should be indexed for efficient polling
    /// - Background job: WHERE ProcessedOnUtc IS NULL ORDER BY CreatedOnUtc
    /// </summary>
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời điểm message được publish thành công (UTC)
    /// 
    /// Mục đích:
    /// - Đánh dấu khi message đã gửi to Kafka
    /// - Null = Chưa gửi, Có giá trị = Đã gửi thành công
    /// - Background job sử dụng để track progress
    /// 
    /// Kiểu dữ liệu:
    /// - DateTime? (nullable)
    /// - Null: Chưa process
    /// - Có giá trị: Đã publish successfully
    /// 
    /// Logic:
    /// - Initially: ProcessedOnUtc = null
    /// - Khi gửi thành công: ProcessedOnUtc = DateTime.UtcNow
    /// - Retry: Nếu lỗi, ProcessedOnUtc giữ null → retry again
    /// - Cleanup: Có thể delete old processed messages sau N days
    /// 
    /// Query:
    /// - Tìm unprocessed: WHERE ProcessedOnUtc IS NULL
    /// - Tìm processed: WHERE ProcessedOnUtc IS NOT NULL
    /// - Tìm old processed: WHERE ProcessedOnUtc < DateTime.UtcNow.AddDays(-30)
    /// </summary>
    public DateTime? ProcessedOnUtc { get; set; }

    /// <summary>
    /// Error message nếu publish failed
    /// 
    /// Mục đích:
    /// - Record error message khi publish fail
    /// - Dùng để debugging: Biết fail vì lý do gì
    /// - Null = Success hoặc belum try, Có giá trị = Error
    /// 
    /// Kiểu dữ liệu:
    /// - string? (nullable, can be NULL)
    /// - Null: No error (success or not processed yet)
    /// - Có giá trị: Error message from Kafka/network/etc
    /// 
    /// Ví dụ error messages:
    /// - "Connection timeout after 30s"
    /// - "Topic not found: todo.created"
    /// - "Invalid message format"
    /// - "Kafka broker unreachable"
    /// 
    /// Retry Strategy:
    /// - Khi error xảy ra:
    ///   1. Save error message
    ///   2. Keep ProcessedOnUtc = null
    ///   3. Increment retry count (ngoài entity, ở config)
    /// - Next background job run: Retry publishing
    /// - After max retries: Mark as failed, alert admin
    /// 
    /// Monitoring:
    /// - Alert khi Error != NULL lâu (> 1 hour)
    /// - Dashboard: Show failed messages count
    /// - Manual retry: Admin endpoint để retry failed messages
    /// </summary>
    public string? Error { get; set; }
}