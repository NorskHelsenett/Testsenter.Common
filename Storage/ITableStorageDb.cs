using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Common.Storage
{
    public interface ITableStorageDb<TEntityType> : IDisposable where TEntityType : TableEntity, new()
    {
        IEnumerable<TEntityType> Get(string partionkey);
        IEnumerable<TEntityType> Take(string partionkey, int take);
        IEnumerable<TEntityType> Query(string partionkey, string propertyName, string equals, int? take = null);
        Task<IEnumerable<TEntityType>> Query(string partionkey, string propertyName, int equals, int? take = null);
        TEntityType Get(TEntityType item, bool throwException = false);
        TEntityType Get(string partionkey, string rowKey, bool throwException = false);
        Task<TEntityType> GetAsync(string partionkey, string rowKey, bool throwException = false);
        IEnumerable<TCustom> GetCustomType<TCustom>(string partionkey) where TCustom : TableEntity, new();
        TableResult Insert(TEntityType item, bool throwExceptionOnInvalidItems, bool replace = false);
        Task<TableResult> InsertAsync(TEntityType item, bool throwExceptionOnInvalidItems, bool replace = false);
        IEnumerable<TEntityType> Insert(IEnumerable<TEntityType> items, bool throwExceptionOnInvalidItems, bool replace = false);
        void DeleteMany(IEnumerable<TEntityType> items);
        IEnumerable<TEntityType> QuerySegmented(string partitionKey, ref TableQuery<TEntityType> query, ref TableContinuationToken continuationToken);
    }
}
