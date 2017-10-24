using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Common.Storage;
using Shared.Common.Storage.Queue;

namespace Shared.AzureCommon.Queue
{
    public class AzureQueueMock : IQueue
    {
        public QueueItem MessageToPop { get; set; }
        public List<QueueItem> Sent { get; set; }

        public string GetName()
        {
            return "AzureQueueMock";
        }

        public QueueItem PopNextMessage()
        {
            return MessageToPop != null || Sent == null ? MessageToPop : Sent.FirstOrDefault();
        }

        public string PopNextMessageString()
        {
            return MessageToPop != null || Sent == null ? MessageToPop?.Content : Sent.FirstOrDefault()?.Content;
        }

        public Task<bool> PopNextMessage(Func<QueueItem, bool> messageHandler)
        {
            return Task.Run(() => messageHandler(MessageToPop));
        }

        public Task<bool> SendAsync(IQueueItem message)
        {
            return Task.Run(() =>
            {
                SendSync(message);
                return true;
            });
        }

        public void SendSync(IQueueItem message)
        {
            if (Sent == null)
                Sent = new List<QueueItem>();

            Sent.Add(AzureQueue.GetContentAsQueueItem(message));
        }
    }
}
