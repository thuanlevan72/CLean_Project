using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IRateLimitService
    {
        // Trả về true nếu được phép đi tiếp, false nếu đã vượt quá giới hạn
        Task<bool> IsAllowedAsync(string key, int maxRequests, TimeSpan window);
    }
}
