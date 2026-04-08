using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Postgres.Data;
using Infrastructure.Postgres.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Repositories;

// Kế thừa GenericRepository để lấy sẵn các hàm Add, Update, Delete, GetById...
public class OutboxRepository : GenericRepository<OutboxMessage, Guid>, IOutboxRepository
{
    // Giả sử GenericRepository của bạn nhận vào DbContext ở hàm khởi tạo
    public OutboxRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int count, CancellationToken ct = default)
    {
        // Trong GenericRepository thường sẽ có biến _context hoặc _dbSet được set là protected.
        // Dưới đây tôi dùng _context, nếu base class của bạn dùng tên khác (ví dụ _dbContext) thì bạn đổi lại nhé.
        return await _context.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.CreatedOnUtc) // Gửi theo thứ tự tạo
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task UpdateMessageAsync(OutboxMessage message, CancellationToken ct = default)
    {
        // Mặc dù GenericRepository có thể đã có hàm Update(), nhưng ta viết riêng hàm này 
        // để ép buộc việc SaveChangesAsync diễn ra ngay lập tức cho Worker.
        _context.Set<OutboxMessage>().Update(message);
        await _context.SaveChangesAsync(ct);
    }
}