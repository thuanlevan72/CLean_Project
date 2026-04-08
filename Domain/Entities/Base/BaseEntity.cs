using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Base
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }


}
