using Microsoft.AspNetCore.Diagnostics;

namespace Api.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // 1. GHI LOG RA FILE (Chỉ developer và sysadmin mới thấy)
            // Log luôn cả Exception (Stack trace chi tiết) để dễ debug
            _logger.LogError(exception, "LỖI HỆ THỐNG: {Message}", exception.Message);

            // 2. TRẢ VỀ CHO NGƯỜI DÙNG (Giấu tịt lỗi thật, chỉ hiện câu chung chung)
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = 500,
                Message = "Hệ thống đang gặp sự cố. Vui lòng thử lại sau!",
                // Bắt buộc KHÔNG CÓ StackTrace ở đây
            };

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            // Trả về true để báo cho .NET biết: "Tôi đã xử lý lỗi này rồi, đừng quăng lỗi gốc ra ngoài nữa"
            return true;
        }
    }
}
