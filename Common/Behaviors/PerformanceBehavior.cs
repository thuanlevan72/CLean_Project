using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Behaviors
{
    // Bắt buộc mọi Request chạy qua đây trước khi vào Handler
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;

        public PerformanceBehavior(ILogger<TRequest> logger)
        {
            _timer = new Stopwatch();
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            // Cho phép request đi tiếp vào Handler thật (ví dụ: CreateTodoCommandHandler)
            var response = await next();

            _timer.Stop();
            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            // KIỂM SOÁT HIỆU NĂNG: Nếu chạy quá 500ms -> Báo động đỏ ra file Log!
            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogWarning(
                    "[PERFORMANCE_WARNING] Tính năng {Name} chạy quá chậm! Thời gian: {ElapsedMilliseconds} ms. Dữ liệu Request: {@Request}",
                    requestName, elapsedMilliseconds, request);
            }

            return response;
        }
    }
}
