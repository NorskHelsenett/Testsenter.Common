using System.Collections.Generic;

namespace Shared.Common.Storage
{
    public interface ISyncJsonStorage<T> where T : IJsonStorageEntity
    {
        T Get(string partitionkey, string rowkey, bool throwExceptionIfNotFound = true);
        IEnumerable<T> Get(string partitionkey);
        bool Post(T element, bool setActive = false);
        bool Put(T element, bool setActive = false);

        bool Delete(T element);
    }
}
