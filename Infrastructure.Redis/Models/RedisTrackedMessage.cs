using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Redis.Models
{
    public class RedisTrackedMessage
    {
        public Guid MessageId { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;

        // Trạng thái: "Sent" (Đã gửi) hoặc "Processed" (Đã xử lý xong)
        public string Status { get; set; } = string.Empty;

        public DateTime TimestampUtc { get; set; }
        public string? Error { get; set; }
    }
}
