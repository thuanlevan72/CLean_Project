using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Base
{
    public interface ISoftDelete
    {
        public DateTimeOffset? SoftDelete { get; set; }
    }
}
