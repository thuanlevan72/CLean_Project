/// <summary>
/// Todo API Application - Entry Point
/// 
/// Mô tả:
/// - Khởi tạo ASP.NET Core Web API cho hệ thống quản lý công việc (Todo Management System)
/// - Cấu hình Dependency Injection (DI) container cho tất cả các tầng: Infrastructure, Application, API
/// - Thiết lập middleware pipeline để xử lý request/response
/// - Khởi động Hangfire background job processing
/// 
/// Kiến trúc 4 tầng (Clean Architecture):
/// 1. Domain Layer: Các entity, interface repository, các quy tắc kinh doanh
/// 2. Application Layer: CQRS handlers, services, DTOs, mapping
/// 3. Infrastructure Layer: DbContext, caching (Redis), messaging (Kafka), background jobs (Hangfire)
/// 4. API Layer: Controllers, middleware, extensions
/// 
/// Công nghệ sử dụng:
/// - Database: PostgreSQL với Entity Framework Core
/// - Caching: Redis cho phân tán cache
/// - Message Queue: Apache Kafka cho event-driven architecture
/// - Background Jobs: Hangfire cho xử lý công việc theo lịch trình
/// - Logging: Serilog cho structured logging
/// - Authentication: JWT token-based authentication
/// </summary>

using Api.Extensions;
using Application;
using Application.DependencyInjections;
using Hangfire;
using Infrastructure.Hangfire;
using Infrastructure.Kafka;
using Infrastructure.Postgres;
using Infrastructure.Redis;
using Serilog;

/// <summary>
/// Cấu hình logger bootstrap - Serilog
/// 
/// Mục đích: Ghi lại các lỗi sơ khai xảy ra trước khi logger chính được khởi tạo hoàn toàn.
/// Sinks: Ghi log ra console trong quá trình khởi động
/// </summary>
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    Log.Information("🚀 Đang khởi động Todo API Server...");

    /// <summary>
    /// Khởi tạo WebApplicationBuilder
    /// 
    /// CreateBuilder(args):
    /// - Tự động nạp appsettings.json từ thư mục hiện tại
    /// - Nạp biến môi trường từ hệ thống
    /// - Nạp User Secrets (nếu chạy Development)
    /// - Thiết lập logging mặc định
    /// </summary>
    var builder = WebApplication.CreateBuilder(args);

    /// <summary>
    /// Cấu hình Serilog như HostBuilder
    /// 
    /// Chức năng:
    /// - ReadFrom.Configuration: Đọc cấu hình Serilog từ appsettings.json
    /// - Cho phép ghi log có cấu trúc (structured logging) với các thuộc tính (properties)
    /// - Hỗ trợ nhiều sinks (console, file, database, etc.)
    /// 
    /// Ưu điểm:
    /// - Khi có lỗi trong Application, log sẽ được ghi chi tiết với stack trace
    /// - Có thể query logs bằng các công cụ như Seq, Splunk
    /// </summary>
    builder.Host.UseSerilog((context, loggerConfiguration) => 
        loggerConfiguration.ReadFrom.Configuration(context.Configuration));

    // ============================================
    // PHASE 1: NẠP HẠ TẦNG (INFRASTRUCTURE LAYER)
    // ============================================
    /// <summary>
    /// Đăng ký các dịch vụ Infrastructure:
    /// 
    /// 1. AddInfrastructurePostgres:
    ///    - Đăng ký DbContext cho Entity Framework Core
    ///    - Cấu hình connection string từ appsettings
    ///    - Thiết lập connection pooling, migrations
    ///    - Đăng ký Repository pattern implementations
    ///
    /// 2. AddInfrastructureRedis:
    ///    - Đăng ký Redis connection factory
    ///    - Cấu hình cache service cho phân tán caching
    ///    - Hỗ trợ Pub/Sub messaging
    ///    - Cấu hình TTL (Time To Live) mặc định
    ///
    /// 3. AddInfrastructureKafka:
    ///    - Đăng ký Kafka producer cho publishing events
    ///    - Cấu hình topic configuration
    ///    - Thiết lập serialization (JSON)
    ///    - Hỗ trợ event-driven architecture
    ///
    /// 4. AddInfrastructureHangfire:
    ///    - Đăng ký Hangfire storage backend (PostgreSQL)
    ///    - Cấu hình Hangfire options (job expiration, cleanup)
    ///    - Prepare database schema cho Hangfire jobs
    ///
    /// 5. AddHangfireServerAndJobs:
    ///    - Khởi tạo Hangfire server để xử lý background jobs
    ///    - Đăng ký các recurring jobs (chạy theo lịch trình định kỳ)
    ///    - Ví dụ: cleanup expired tokens, send email reminders, etc.
    /// </summary>
    builder.Services.AddInfrastructurePostgres(builder.Configuration);
    builder.Services.AddInfrastructureRedis(builder.Configuration);
    builder.Services.AddInfrastructureKafka(builder.Configuration);
    builder.Services.AddInfrastructureHangfire(builder.Configuration);
    builder.Services.AddHangfireServerAndJobs();

    // ============================================
    // PHASE 2: NẠP LÕI (APPLICATION LAYER)
    // ============================================
    /// <summary>
    /// Đăng ký Application layer dependencies:
    /// 
    /// - MediatR: CQRS pattern implementation
    ///   + Quản lý Commands (thay đổi dữ liệu) và Queries (lấy dữ liệu)
    ///   + Mỗi Command/Query có Handler riêng xử lý logic
    ///   + Middleware chain: Validation → Logging → Caching → Handler
    ///
    /// - FluentValidation: Input validation
    ///   + Validators cho mỗi Command/Query
    ///   + Validate tự động qua MediatR pipeline
    ///   + Trả về chi tiết lỗi nếu validation thất bại
    ///
    /// - AutoMapper: Object mapping
    ///   + Mapping giữa DTOs ↔ Domain Entities
    ///   + Mapping từ Commands → Entities
    ///   + Giảm code boilerplate
    ///
    /// - Application Services: Business logic implementations
    ///   + Todo services, Category services, Tag services
    ///   + Authentication & Authorization services
    ///   + Cache service wrappers
    /// </summary>
    builder.Services.AddApplication();

    // ============================================
    // PHASE 3: NẠP GIAO DIỆN (API LAYER)
    // ============================================
    /// <summary>
    /// Đăng ký API layer dependencies được ẩn trong extension method:
    /// 
    /// Nội dung của AddApiServices (xem Api/Extensions/ApiServiceCollectionExtensions.cs):
    ///
    /// 1. Swagger/OpenAPI:
    ///    - Tự động generate API documentation
    ///    - Cho phép test API trực tiếp từ UI
    ///    - Hiển thị JWT authorization requirement
    ///
    /// 2. JWT Authentication:
    ///    - Cấu hình JWT bearer token validation
    ///    - Đọc secret key từ appsettings
    ///    - Xác minh chữ ký token
    ///    - Claim-based authorization
    ///
    /// 3. CORS (Cross-Origin Resource Sharing):
    ///    - Cho phép request từ frontend clients
    ///    - Cấu hình allowed origins, methods, headers
    ///    - Xử lý preflight requests
    ///
    /// 4. MVC Controllers:
    ///    - Đăng ký service cho API controllers
    ///    - Model binding configuration
    ///    - JSON serializer options (camelCase, null handling)
    ///
    /// 5. Global Exception Handler Middleware:
    ///    - Catch unhandled exceptions
    ///    - Return consistent error responses
    ///    - Log errors tự động
    /// </summary>
    builder.Services.AddApiServices(builder.Configuration);

    /// <summary>
    /// Xây dựng WebApplication
    /// 
    /// Chuyển WebApplicationBuilder thành WebApplication
    /// Tất cả DI services đã được đăng ký vào container
    /// </summary>
    var app = builder.Build();

    // ============================================
    // PHASE 4: KÍCH HOẠT MIDDLEWARE PIPELINE
    // ============================================
    /// <summary>
    /// Middleware pipeline - thứ tự rất quan trọng!
    /// Request đi từ trên xuống, Response đi từ dưới lên
    /// 
    /// 1. UseSerilogRequestLogging:
    ///    - Ghi lại tất cả HTTP requests/responses
    ///    - Ghi method, path, status code, duration
    ///    - Hữu ích để debugging và monitoring
    ///
    /// 2. UseHttpsRedirection:
    ///    - Redirect HTTP → HTTPS (security best practice)
    ///    - Kỳ loại: chỉ hoạt động trong Production
    ///
    /// 3. UseCors:
    ///    - Xử lý CORS headers
    ///    - Phải nằm trước Authentication/Authorization
    ///    - Cho phép frontend requests
    ///
    /// 4. UseAuthentication:
    ///    - Validate JWT token từ Authorization header
    ///    - Khôi phục User principal từ token claims
    ///    - Phải nằm trước Authorization
    ///
    /// 5. UseAuthorization:
    ///    - Kiểm tra [Authorize] attributes
    ///    - Kiểm tra roles, policies
    ///    - Phải nằm sau Authentication
    /// </summary>
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();

    // ============================================
    // PHASE 5: CẤU HÌNH HANGFIRE & SWAGGER
    // ============================================
    /// <summary>
    /// Hangfire Dashboard:
    /// - URL: /hangfire (accessible tại http://localhost:5000/hangfire)
    /// - Cho phép xem và quản lý background jobs
    /// - Xem recurring jobs, job execution history
    /// - Manual job triggering và retry
    ///
    /// InitializeHangfireJobs:
    /// - Đăng ký tất cả recurring jobs
    /// - Ví dụ: cleanup expired sessions, send reminders, etc.
    /// - Tự động chạy theo cron schedule
    /// </summary>
    app.UseHangfireDashboard("/hangfire");
    app.InitializeHangfireJobs();

    /// <summary>
    /// Development-only Swagger UI:
    /// 
    /// Khi chạy trong Development environment:
    /// - Enable Swagger middleware (xử lý /swagger/v1/swagger.json)
    /// - Enable SwaggerUI (xử lý /swagger/index.html - interactive API explorer)
    ///
    /// Production: Swagger bị tắt để không expose API details
    /// </summary>
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    /// <summary>
    /// MapControllers:
    /// - Tự động route requests đến controllers dựa trên [Route] attributes
    /// - Ví dụ: [Route("api/[controller]")] → /api/todositems
    /// - Phải nằm sau tất cả middleware configuration
    /// </summary>
    app.MapControllers();

    /// <summary>
    /// Khởi động ứng dụng:
    /// - Bắt đầu Kestrel web server
    /// - Listen trên URLs được cấu hình (appsettings)
    /// - Default: http://localhost:5000, https://localhost:5001
    /// </summary>
    app.Run();
}
catch (Exception ex)
{
    /// <summary>
    /// Global exception handler:
    /// - Catch bất kỳ unhandled exception nào
    /// - Ghi lại fatal error vào logs
    /// - Kết thúc ứng dụng
    /// 
    /// Tình huống phổ biến:
    /// - Connection string không đúng → failed to connect to database
    /// - Port đã được sử dụng → failed to start Kestrel
    /// - Missing dependencies → failed to resolve services
    /// </summary>
    Log.Fatal(ex, "💥 API bị sập ngay lúc khởi động!");
}
finally
{
    /// <summary>
    /// Cleanup:
    /// - Flush tất cả pending logs trước khi shutdown
    /// - Đóng Serilog logger gracefully
    /// - Giải phóng resources (connections, file handles)
    /// </summary>
    Log.CloseAndFlush();
}