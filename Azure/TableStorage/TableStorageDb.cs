using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using Newtonsoft.Json;
using Shared.Common.Storage;

namespace Shared.AzureCommon.TableStorage
{
    public class TableStorageDb<TEntityType> : ITableStorageDb<TEntityType> where TEntityType : TableEntity, new() 
    {
        #region properties

        private CloudStorageAccount _storageAccount;
        private CloudStorageAccount StorageAccount
        {
            get
            {
                if (_storageAccount == null)
                    _storageAccount = CloudStorageAccount.Parse(_connectionString);

                return _storageAccount;
            }
        }

        int _hoursToKeepCache = 2;
        DateTime? _lastUpdated = null;
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object ClientLock = new object();
        private CloudTableClient _tableClient;
        private CloudTableClient TableClient
        {
            get
            {
                lock (ClientLock)
                {
                    if (_lastUpdated != null && _tableClient != null)
                    {
                        if (_lastUpdated.Value.AddHours(_hoursToKeepCache).CompareTo(DateTime.Now) < 0)
                            _tableClient = null;
                        else
                            return _tableClient;
                    }

                    if (_tableClient == null)
                        _tableClient = StorageAccount.CreateCloudTableClient();

                    return _tableClient;
                }
            }
        }

        private CloudTable GetTable()
        {
            var table = TableClient.GetTableReference(TableName);
            table.CreateIfNotExists();

            return table;
        }

        #endregion

        private readonly string _connectionString;
        public string TableName;
        private const int MaxInsertAzureCount = 99;

        public TableStorageDb(string connectionString, Enum name)
            : this(connectionString, name.ToString())
        {
        }

        public TableStorageDb(string connectionString, string tableName)
        {
            if (string.IsNullOrEmpty(connectionString))
                // ReSharper disable once NotResolvedInText
                throw new ArgumentNullException("Connectionstring var null");

            _connectionString = connectionString;
            TableName = tableName.ToString().ToLower();
        }

        public virtual IEnumerable<TEntityType> Get()
        {
            return Execute(new TableQuery<TEntityType>());
        }

        public async Task<IEnumerable<TEntityType>> GetAsync()
        {
            return await ExecuteAsync(new TableQuery<TEntityType>());
        }


        public virtual IEnumerable<TEntityType> Get(string partionkey)
        {
            var operation = new TableQuery<TEntityType>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partionkey));

            return Execute(operation);
        }

        public async Task<IEnumerable<TEntityType>> GetAsync(string partionkey)
        {
            var operation = new TableQuery<TEntityType>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partionkey));

            return await ExecuteAsync(operation);
        }

        public IEnumerable<TEntityType> Take(int take)
        {
            return Execute(new TableQuery<TEntityType>().Take(take));
        }

        public IEnumerable<TEntityType> Take(string partionkey, int take)
        {
            var operation = new TableQuery<TEntityType>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partionkey))
                .Take(take);

            var result = Execute(operation);
            return result;
        }

        public async Task<IEnumerable<TEntityType>> TakeAsync(int take)
        {
            return await ExecuteAsync(new TableQuery<TEntityType>().Take(take));
        }

        public async Task<IEnumerable<TEntityType>> TakeAsync(string partionkey, int take)
        {
            var operation = new TableQuery<TEntityType>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partionkey))
                .Take(take);

            return await ExecuteAsync(operation);
        }

        public async Task<IEnumerable<TEntityType>> Query(string partionkey, string propertyName, int equals, int? take = null)
        {
            var filter = TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partionkey),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForInt(propertyName, QueryComparisons.Equal, equals));

            var operation = take != null
                ?
                    new TableQuery<TEntityType>()
                        .Where(filter)
                        .Take(take)
                :
                    new TableQuery<TEntityType>()
                        .Where(filter);

            var x = await ExecuteAsync(operation);
            return x;
        }

        public IEnumerable<TEntityType> Query(string partionkey, string propertyName, string equals, int? take = null)
        {
            var filter = TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partionkey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, equals));

            var operation = take != null
                ?
                    new TableQuery<TEntityType>()
                        .Where(filter)
                        .Take(take)
                :
                    new TableQuery<TEntityType>()
                        .Where(filter);
            var x = Execute(operation);
            return x;
        }

        public TEntityType Get(TEntityType item, bool throwException = false)
        {
            return Get(item.PartitionKey, item.RowKey, throwException);
        }

        public virtual TEntityType Get(string partionkey, string rowKey, bool throwException = false)
        {
            var operation = TableOperation.Retrieve<TEntityType>(partionkey, rowKey);
            var result = Execute(operation).Result;

            if (result == null)
                if(throwException)
                // ReSharper disable once UseStringInterpolation
                    throw new ArgumentException(string.Format("Could not find configuration item with rowkey={0} and partitionkey={1}", rowKey, partionkey));
                else
                {
                    return null;
                }
            return (TEntityType) result.Result;
        }

        public async Task<TEntityType> GetAsync(string partionkey, string rowKey, bool throwException = false)
        {
            try
            {
                var operation = TableOperation.Retrieve<TEntityType>(partionkey, rowKey);
                var result = await Execute(operation);

                return (TEntityType) result.Result;
            }
            catch (Exception)
            {
                if (throwException)
                    throw;

                return null;
            }
        }

        public IEnumerable<TCustom> GetCustomType<TCustom>(string partionkey) where TCustom : TableEntity, new() 
        {
            var operation = new TableQuery<TCustom>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partionkey));

            return GetTable().ExecuteQuery(operation);
        }

        public TableResult Insert(TEntityType item, bool throwExceptionOnInvalidItems, bool replace = false)
        {
            var operation = replace ? TableOperation.InsertOrReplace(item) :
                                      TableOperation.Insert(item);
            try
            {
                return Execute(operation).Result;
            }
            catch (Exception c)
            {
                if (c.InnerException == null)
                    throw;

                if (c.InnerException.Message.Contains("409") || c.Message.Contains("409"))
                    throw new ArgumentException("Table storage returned 409: Element with rowKey: " + item.RowKey + " and partitionkey: " + item.PartitionKey + " already exists in table " + TableName);

                if (c.InnerException.Message.Contains("400") || c.Message.Contains("400"))
                    throw new ArgumentException("Table storage returned 400: Element contains invalid values: " + JsonConvert.SerializeObject(item));

                throw;
            }
        }

        public async Task<TableResult> InsertAsync(TEntityType item, bool throwExceptionOnInvalidItems, bool replace = false)
        {
            var operation = replace ? TableOperation.InsertOrReplace(item) :
                                      TableOperation.Insert(item);
            try
            {
                return await Execute(operation);
            }
            catch (Exception c)
            {
                if (c.InnerException == null)
                    throw;

                if (c.InnerException.Message.Contains("409") || c.Message.Contains("409"))
                    throw new ArgumentException("Table storage returned 409: Element with rowKey: " + item.RowKey + " and partitionkey: " + item.PartitionKey + " already exists in table " + TableName);

                if (c.InnerException.Message.Contains("400") || c.Message.Contains("400"))
                    throw new ArgumentException("Table storage returned 400: Element contains invalid values: " + JsonConvert.SerializeObject(item));

                throw;
            }
        }

        public IEnumerable<TEntityType> Insert(IEnumerable<TEntityType> items, bool throwExceptionOnInvalidItems, bool replace = false)
        {
            var list = items.ToList();

            if (list.Count() > MaxInsertAzureCount)
                return InsertMany(list, throwExceptionOnInvalidItems, replace);

            return InsertIt(list, throwExceptionOnInvalidItems, replace);
        }

        private IEnumerable<TEntityType> InsertMany(IEnumerable<TEntityType> items, bool throwExceptionOnInvalidItems, bool replace = false)
        {
            var list = items.ToList();
            var result = new List<TEntityType>();

            var hasMore = true;
            int skippy = 0;
            int totalCount = 0;
            while (hasMore)
            {
                var range = list.Skip(skippy).Take(MaxInsertAzureCount).ToList();
                var thisResult = InsertIt(range, throwExceptionOnInvalidItems, replace);
                skippy += MaxInsertAzureCount;
                totalCount += range.Count();

                result.AddRange(thisResult);

                if (range.Count() < MaxInsertAzureCount || totalCount == list.Count())
                    hasMore = false;
            }

            if (result.Count != list.Count())
                throw new ArgumentException("Fail in InsertMany: inserted was: " + totalCount + ", while required was: " + list.Count());

            return result;
        }

        private IEnumerable<TEntityType> InsertIt(IEnumerable<TEntityType> items, bool throwExceptionOnInvalidItems, bool replace)
        {
            var list = items.ToList();

            CheckValid(list, throwExceptionOnInvalidItems);
            var tableBatchOperation = new TableBatchOperation();

            foreach (var item in list)
            {
                if (replace)
                    tableBatchOperation.Add(TableOperation.InsertOrReplace(item));
                else
                    tableBatchOperation.Add(TableOperation.Insert(item));
            }

            var result = Execute(tableBatchOperation).Result;
            var projected = result.Select(x => (TEntityType)x.Result);

            return projected;
        }

        public void DeleteMany(IEnumerable<TEntityType> items)
        {
            var list = items.ToList();

            if (list.Count == 0)
                return;

            var hasMore = true;
            int skippy = 0;
            int totalCount = 0;
            while (hasMore)
            {
                var range = list.Skip(skippy).Take(MaxInsertAzureCount).ToList();
                DeleteIt(range);
                skippy += MaxInsertAzureCount;
                totalCount += range.Count;

                if (range.Count < MaxInsertAzureCount || totalCount == list.Count)
                    hasMore = false;
            }
        }

        private void DeleteIt(IEnumerable<TEntityType> items)
        {
            var tableBatchOperation = new TableBatchOperation();
            foreach (var item in items.Where(t => t != null))
            {
                item.ETag = "*";
                tableBatchOperation.Add(TableOperation.Delete(item));
            }

            if (!tableBatchOperation.Any())
                return;

            // ReSharper disable once UnusedVariable
            var result = Execute(tableBatchOperation).Result;
        }

        public IEnumerable<TEntityType> QuerySegmented(string partitionKey, ref TableQuery<TEntityType> query, ref TableContinuationToken continuationToken)
        {
            if (query == null)
                query =
                   (from element in GetTable().CreateQuery<TEntityType>()
                    where element.PartitionKey == partitionKey
                    select element)
                    .AsTableQuery();

            var queryResult = query.ExecuteSegmented(continuationToken);
            continuationToken = queryResult.ContinuationToken;

            return queryResult;
        }

        #region Helpers
        private async Task<TableResult> Execute(TableOperation tableOperation)
        {
            return await GetTable().ExecuteAsync(tableOperation).ConfigureAwait(false);
        }

        private async Task<IList<TableResult>> Execute(TableBatchOperation tableBatchOperation)
        {
            return await GetTable().ExecuteBatchAsync(tableBatchOperation).ConfigureAwait(false);
        }

        private async Task<IEnumerable<TEntityType>> ExecuteAsync(TableQuery<TEntityType> query)
        {
            var items = new List<TEntityType>();
            TableContinuationToken token = null;

            do
            {

                TableQuerySegment<TEntityType> seg = await GetTable().ExecuteQuerySegmentedAsync(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);

            } while (token != null);

            return items;
        }

        private IEnumerable<TEntityType> Execute(TableQuery<TEntityType> query)
        {
            return GetTable().ExecuteQuery(query);
        }

        // ReSharper disable once UnusedParameter.Local
        private void CheckValid(IEnumerable<TEntityType> items, bool throwException)
        {
            var invalid = items.Where(item => item.PartitionKey == null || item.RowKey == null);
            if (!invalid.Any())
                return;

            if (throwException)
                throw new ArgumentException("One or more configurationItems are invalid");
        }

        #endregion

        public void Dispose()
        {
            _storageAccount = null;
            _tableClient = null;
        }
    }
}
