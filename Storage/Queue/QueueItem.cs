
using System;
using log4net;
using Newtonsoft.Json;

namespace Shared.Common.Storage.Queue
{
    public class QueueItem
    {
        public QueueItem() { }

        public int QueueItemType { get; set; }
        public string Content { get; set; }

        public QueueItemType GetQueueItemType()
        {
            return (QueueItemType) QueueItemType;
        }

        public QueueItem(QueueItemType itemType)
        {
            QueueItemType = (int) itemType;
        }

        public string GetContentAsJson()
        {
            return Content;
        }

        public static QueueItem GetQueueItem(string msg, ILog log)
        {
            string failMessage;
            var obj = TryDeserialize(msg, out failMessage);
            if (obj != null)
                return obj;

            try
            {
                var start = msg.IndexOf("<Message>", StringComparison.Ordinal) + 10;
                var stop = msg.IndexOf("</Message>", StringComparison.Ordinal);
                msg = msg.Substring(start - 1, stop - start + 1);

                var strong = TryDeserialize(msg, out failMessage);
                if (strong != null)
                    return strong;
            }
            catch (Exception e)
            {
                if (log != null)
                    log.Error("Fail when deserializing: " + failMessage + ", and: " + e.Message);
                else
                    throw;
            }

            return null;
        }

        private static QueueItem TryDeserialize(string msg, out string failmessage)
        {
            failmessage = "";

            try
            {
                return JsonConvert.DeserializeObject<QueueItem>(msg);
            }
            catch (Exception e)
            {
                failmessage = "Fail when deserializing: " + e.Message;
                return null;
            }
        }
    }
}
