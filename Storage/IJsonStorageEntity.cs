
namespace Shared.Common.Storage
{
    public interface IJsonStorageEntity
    {
        string GetPartitionKey();
        string GetRowKey();
    }
}
