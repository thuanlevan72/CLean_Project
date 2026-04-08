using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IKafkaPublisher
    {
        // Gửi tin nhắn cơ bản
        Task PublishAsync<TValue>(string topic, TValue message, CancellationToken ct = default);

        // Gửi tin nhắn có Key (Đảm bảo thứ tự cho cùng 1 thực thể, VD: cùng 1 UserId)
        Task PublishWithKeyAsync<TKey, TValue>(string topic, TKey key, TValue message, CancellationToken ct = default);
    }
}
