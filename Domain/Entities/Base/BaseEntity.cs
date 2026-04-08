using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Base
{
    /// <summary>
    /// BaseEntity: Lớp cơ sở cho tất cả domain entities
    /// 
    /// Mô tả:
    /// - Định nghĩa các thuộc tính audit chung cho tất cả entities
    /// - Hỗ trợ soft delete (đánh dấu xóa thay vì xóa vật lý)
    /// - Theo dõi thông tin người tạo, người sửa, người xóa
    /// - Generic với TId cho phép sử dụng các kiểu ID khác nhau (Guid, int, string)
    /// 
    /// Sử dụng:
    /// - Các entity khác kế thừa từ BaseEntity<Guid> hoặc BaseEntity<int>
    /// - EF Core sẽ tự động quản lý các thuộc tính này
    /// - Có thể filter soft-deleted items trong queries
    /// 
    /// Ví dụ:
    /// public class TodoItem : BaseEntity<Guid>
    /// {
    ///     public string Title { get; set; }
    ///     // ... other properties
    /// }
    /// </summary>
    public abstract class BaseEntity<TId>
    {
        /// <summary>
        /// Khóa chính (Primary Key)
        /// 
        /// Mục đích:
        /// - Định danh duy nhất cho mỗi entity
        /// - Được EF Core tự động mapping tới cột Id trong database
        /// 
        /// Kiểu dữ liệu:
        /// - TId: Có thể là Guid, int, string, tùy theo entity
        /// - default!: C# 8+ null-forgiving operator
        /// </summary>
        public TId Id { get; set; } = default!;

        /// <summary>
        /// Metadata Tạo (Creation Audit)
        /// 
        /// CreatedAt:
        /// - Thời điểm tạo entity (UTC)
        /// - Set tự động khi entity được tạo trong DbContext
        /// - Dùng để sắp xếp, filter theo thời gian tạo
        ///
        /// CreatedBy:
        /// - Định danh người tạo entity (User ID hoặc username)
        /// - Nullable vì có thể tạo từ system (null) hoặc user (có giá trị)
        /// - Dùng để audit trail, biết ai tạo entity nào
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Metadata Sửa (Update Audit)
        /// 
        /// UpdatedAt:
        /// - Thời điểm sửa gần nhất (UTC)
        /// - Nullable vì entity có thể chưa bao giờ được sửa (null = chỉ được tạo)
        /// - Dùng để biết entity nào được thay đổi gần đây
        ///
        /// UpdatedBy:
        /// - Định danh người sửa gần nhất
        /// - Nullable như UpdatedAt
        /// - Dùng để audit trail, biết ai sửa entity lần cuối
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Metadata Xóa (Soft Delete Audit)
        /// 
        /// IsDeleted:
        /// - Cờ (flag) để đánh dấu entity đã xóa
        /// - false: entity còn hoạt động (active)
        /// - true: entity đã xóa (inactive)
        /// - Lợi ích: không mất dữ liệu, có thể restore lại
        ///
        /// DeletedAt:
        /// - Thời điểm xóa (UTC)
        /// - Nullable, chỉ có giá trị khi IsDeleted = true
        ///
        /// DeletedBy:
        /// - Định danh người xóa
        /// - Nullable, chỉ có giá trị khi IsDeleted = true
        /// - Dùng để biết ai xóa entity
        ///
        /// Cách sử dụng:
        /// - Khi delete: Set IsDeleted = true, DeletedAt = now, DeletedBy = userId
        /// - Khi query: Luôn thêm filter .Where(x => !x.IsDeleted)
        /// - Khi restore: Set IsDeleted = false, DeletedAt = null, DeletedBy = null
        /// </summary>
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
