using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IPubSubService
    {
        Task PublishAsync(string channel, string message);
        Task SubscribeAsync(string channel, Action<string, string> handler);
    }
}
