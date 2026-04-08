using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories
{
    public interface IOutboxRepository: IGenericRepository<OutboxMessage, Guid>
    {
        // Lấy ra danh sách các tin nhắn CHƯA ĐƯỢC GỬI (ProcessedOnUtc == null)
        Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int count, CancellationToken ct = default);

        // Cập nhật trạng thái (Đã gửi xong, hoặc Cập nhật lỗi)
        Task UpdateMessageAsync(OutboxMessage message, CancellationToken ct = default);
    }
}
