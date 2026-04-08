namespace Application.Interfaces;

public interface ICacheService
{
    // Lấy dữ liệu từ Cache
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    // Lưu dữ liệu vào Cache
    Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default);

    // Xóa dữ liệu khỏi Cache
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    // Hàm "Sát thủ": Kiểm tra Cache, nếu CÓ thì trả về, nếu KHÔNG thì chạy hàm Factory gọi DB -> Lưu Cache -> Trả về
    Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default);

    // Xóa danh sách key theo Pattern (rất hữu ích khi cần xóa hàng loạt Cache của 1 User)
    Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
}