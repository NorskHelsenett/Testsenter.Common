using Newtonsoft.Json;
using System;

namespace Shared.Common.Storage.Queue
{
    public class SimpleQueueItem : IQueueItem
    {
        private readonly QueueItemType _type;

        public SimpleQueueItem(QueueItemType type, string id)
        {
            Id = id;
            _type = type;
        }
        
        public string Id { get; set; }
        public object Content { get; set; }

        public string GetContentAsJson()
        {
            return JsonConvert.SerializeObject(Content);
        }

        public QueueItemType GetQueueItemType()
        {
            return _type;
        }

        [Obsolete("For serialization purposes")]
        public SimpleQueueItem() { }
    }
}
