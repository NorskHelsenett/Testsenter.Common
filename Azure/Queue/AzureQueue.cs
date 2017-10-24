using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Shared.Common.Storage;
using Shared.Common.Storage.Queue;

namespace Shared.AzureCommon.Queue
{
    public class AzureQueue : BaseAzureStorage, IDisposable, IQueue
    {
        private readonly string _queueName;

        public AzureQueue(string connectionString, Enum name)
            : this(connectionString, name.ToString())
        {
        }

        public AzureQueue(string connectionString, string queueName)
            : base(connectionString)
        {
            _queueName = queueName.ToString().ToLower();
        }

        private CloudQueueClient _client;
        protected CloudQueueClient Client
        {
            get
            {
                if (_client == null)
                    _client = StorageAccount.CreateCloudQueueClient();

                return _client;
            }
        }

        private CloudQueue GetQueue()
        {
            CloudQueue queue = Client.GetQueueReference(_queueName);
            queue.CreateIfNotExists();

            return queue;
        }

        public async Task<bool> SendAsync(IQueueItem message)
        {
            var queueMessage = GetContentAsQueueItem(message);
            var content = JsonConvert.SerializeObject(queueMessage);

            await GetQueue().AddMessageAsync(new CloudQueueMessage(content));

            return true;
        }

        public void SendSync(IQueueItem message)
        {
            var queueMessage = GetContentAsQueueItem(message);
            var content = JsonConvert.SerializeObject(queueMessage);

            GetQueue().AddMessage(new CloudQueueMessage(content));
        }

        public QueueItem PopNextMessage()
        {
            var queue = GetQueue();
            var nextMessage = queue.GetMessage();
            if (nextMessage == null)
                return null;

            var content = TryDeserialize(nextMessage.AsString);
            if (content == null)
            {
                queue.DeleteMessage(nextMessage);
                return PopNextMessage();
            }

            queue.DeleteMessage(nextMessage);

            return content;
        }

        public async Task<bool> PopNextMessage(Func<QueueItem, bool> messageHandler)
        {
            var queue = GetQueue();
            var nextMessage = await queue.GetMessageAsync();
            if (nextMessage == null)
                return false;

            var content = TryDeserialize(nextMessage.AsString);
            if(content == null)
            {
                queue.DeleteMessage(nextMessage);
                return true;
            }

            var deleteIt = messageHandler(content);
            if(deleteIt)
                queue.DeleteMessage(nextMessage);

            return true;
        }

        public string PopNextMessageString()
        {
            var queue = GetQueue();
            var nextMessage = queue.GetMessage();
            if (nextMessage == null)
                return null;

            var content = nextMessage.AsString;

            queue.DeleteMessage(nextMessage);

            return content;
        }

        private QueueItem TryDeserialize(string msg)
        {
            try
            {
                return JsonConvert.DeserializeObject<QueueItem>(msg);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            _client = null;
        }

        public static QueueItem GetContentAsQueueItem(IQueueItem message)
        {
            return new QueueItem
            {
                QueueItemType = (int)message.GetQueueItemType(),
                Content = message.GetContentAsJson()
            };
        }

        public string GetName()
        {
            return _queueName;
        }
    }
}
