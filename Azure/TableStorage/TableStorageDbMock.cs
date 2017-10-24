using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Shared.Common.Storage;

namespace Shared.AzureCommon.TableStorage
{
    public class TableStorageDbMock<TEntityType> : ITableStorageDb<TEntityType> where TEntityType : TableEntity, new()
    {
        private Dictionary<string, Dictionary<string, TEntityType>> _storage;
        public Dictionary<string, Dictionary<string, TEntityType>> Storage
        {
            get
            {
                if (_storage == null)
                    _storage = new Dictionary<string, Dictionary<string, TEntityType>>();

                return _storage;
            }
            set
            {
                _storage = value;
            }
        }

        public void DeleteMany(IEnumerable<TEntityType> items)
        {
            foreach (var item in items)
            {
                if (!Storage.ContainsKey(item.PartitionKey))
                    continue;

                if(!Storage[item.PartitionKey].ContainsKey(item.RowKey))
                    continue;

                Storage[item.PartitionKey].Remove(item.RowKey);
            }
        }

        public void Dispose()
        {
        }

        public IEnumerable<TEntityType> Get(string partionkey)
        {
            if (!Storage.ContainsKey(partionkey))
                return new List<TEntityType>();

            return Storage[partionkey].Select(x => x.Value);
        }

        public TEntityType Get(TEntityType item, bool throwException = false)
        {
            return Get(item.PartitionKey, item.RowKey, throwException);
        }

        public TEntityType Get(string partionkey, string rowKey, bool throwException = false)
        {
            var partition = Get(partionkey);
            if (!partition.Any() || !Storage[partionkey].ContainsKey(rowKey))
            {
                if (throwException)
                    throw new Exception("Storage does not contain element with $partionKey and $rowKey");

                return null;
            }

            return Storage[partionkey][rowKey];
        }

        public Task<TEntityType> GetAsync(string partionkey, string rowKey, bool throwException = false)
        {
            return Task.FromResult(Get(partionkey, rowKey, throwException));
        }

        public IEnumerable<TCustom> GetCustomType<TCustom>(string partionkey) where TCustom : TableEntity, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntityType> Insert(IEnumerable<TEntityType> items, bool throwExceptionOnInvalidItems, bool replace = false)
        {
            var result = new List<TEntityType>();

            foreach (var element in items)
            {
                var tableResult = Insert(element, throwExceptionOnInvalidItems, replace);
                if(tableResult != null)
                    result.Add((TEntityType) tableResult.Result);
            }

            return result;
        }

        public TableResult Insert(TEntityType item, bool throwExceptionOnInvalidItems, bool replace = false)
        {
            if(item.RowKey == null || item.PartitionKey == null)
            {
                if(throwExceptionOnInvalidItems)
                    throw new Exception("Missing rowkey, partitionKey or timestamp");

                return null;
            }

            if(Get(item.PartitionKey, item.RowKey) != null)
            {
                if(!replace)
                    throw new Exception("Storage already contain element with $partionKey and $rowKey");

                Storage[item.PartitionKey][item.RowKey] = item;
                return new TableResult() { HttpStatusCode = 201, Result = Storage[item.PartitionKey][item.RowKey] };
            }

            if (!Storage.ContainsKey(item.PartitionKey))
                Storage.Add(item.PartitionKey, new Dictionary<string, TEntityType>());

            Storage[item.PartitionKey].Add(item.RowKey, item);

            return new TableResult() { HttpStatusCode = 201, Result = Storage[item.PartitionKey][item.RowKey] };
        }

        public Task<TableResult> InsertAsync(TEntityType item, bool throwExceptionOnInvalidItems, bool replace = false)
        {
            return Task.FromResult(Insert(item, throwExceptionOnInvalidItems, replace));
        }

        public IEnumerable<TEntityType> Query(string partionkey, string propertyName, string equals, int? take = default(int?))
        {
            var resultSet = new List<TEntityType>();

            if (!Storage.ContainsKey(partionkey))
                return resultSet;

            foreach (var item in Storage[partionkey].Keys)
            {
                var thisItem = Storage[partionkey][item];
                try
                {
                    PropertyInfo propertyInfo = thisItem.GetType().GetProperty(propertyName);
                    var valobject = propertyInfo.GetValue(thisItem, null);

                    var val = valobject.ToString();
                    if(val == equals)
                        resultSet.Add(thisItem);
                }
                catch(Exception) { }
            }

            return resultSet;
        }

        public Task<IEnumerable<TEntityType>> Query(string partionkey, string propertyName, int equals, int? take = default(int?))
        {
            return Task.FromResult(Query(partionkey, propertyName, equals.ToString()));
        }

        public IEnumerable<TEntityType> QuerySegmented(string partitionKey, ref TableQuery<TEntityType> query, ref TableContinuationToken continuationToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntityType> Take(string partionkey, int take)
        {
            throw new NotImplementedException();
        }
    }
}
