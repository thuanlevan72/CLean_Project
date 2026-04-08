using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IDistributedLockService
    {
        Task<bool> AcquireLockAsync(string resourceKey, string token, TimeSpan expiry);
        Task ReleaseLockAsync(string resourceKey, string token);
    }
}
