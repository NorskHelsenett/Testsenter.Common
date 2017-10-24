using Shared.Common.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace Shared.Common.Storage
{
    public interface IQueue
    {
        string GetName();
        Task<bool> SendAsync(IQueueItem message);
        void SendSync(IQueueItem message);
        QueueItem PopNextMessage();
        string PopNextMessageString();
        Task<bool> PopNextMessage(Func<QueueItem, bool> messageHandler);
    }
}
