using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Common.Storage
{
    public interface IJsonStorage<T> where T : IJsonStorageEntity
    {
        Task<T> Get(string partitionkey, string rowkey, bool throwExceptionIfNotFound = true);
        Task<TSuper> GetSuper<TSuper>(string partitionkey, string rowkey, bool throwExceptionIfNotFound = true) where TSuper : T;
        Task<T> TopElement(string partitionkey);
        Task<IEnumerable<T>> Get();
        Task<IEnumerable<T>> Get(string partitionkey);
        Task<IEnumerable<T>> Get(string partitionkey, bool shouldBeActive);
        Task<IEnumerable<T>> Take(int take);
        Task<bool> Post(T element, bool setActive = false);
        Task<bool> Post(IEnumerable<T> elements);
        Task<bool> Put(T element, bool setActive = false);
        Task<bool> Put(IEnumerable<T> elements);
        Task<bool> Delete(string partitionKey, string rowKey);
        Task<bool> Delete(T element);
        Task<bool> Delete(IEnumerable<T> elements);

    }
}
