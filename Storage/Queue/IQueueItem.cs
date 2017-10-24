namespace Shared.Common.Storage.Queue
{
    public interface IQueueItem
    {
        string GetContentAsJson();
        QueueItemType GetQueueItemType();
    }
}
