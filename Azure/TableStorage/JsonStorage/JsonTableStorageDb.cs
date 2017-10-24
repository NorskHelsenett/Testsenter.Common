using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Common.Storage;

namespace Shared.AzureCommon.TableStorage.JsonStorage
{
    public class JsonTableStorageDb<T> : TableStorageDb<JsonStorageEntity<T>>, IJsonStorage<T> where T : class, IJsonStorageEntity
    {
        public JsonTableStorageDb(string connectionString, Enum name) : this(connectionString, name.ToString()) {}

        public JsonTableStorageDb(string connectionString, string tableName) : base(connectionString, tableName) { }

        #region Get
        public new async Task<IEnumerable<T>> Get()
        {
            var result = await GetAsync();
            return result?.Select(x => x.Content);
        }
        public new async Task<T> Get(string partitionkey, string rowkey, bool throwExceptionIfNotFound = true)
        {
            var result = await GetAsync(partitionkey, rowkey, throwExceptionIfNotFound);
            return result?.Content;
        }

        public async Task<TSuper> GetSuper<TSuper>(string partitionkey, string rowkey, bool throwExceptionIfNotFound = true) where TSuper : T
        {
            var result = await GetAsync(partitionkey, rowkey, throwExceptionIfNotFound);
            if (result == null)
                return default(TSuper);

            return JsonConvert.DeserializeObject<TSuper>(result.ContentAsJson);
        }

        public new async Task<IEnumerable<T>> Get(string partitionkey)
        {
            var result = await GetAsync(partitionkey);
            return result.Select(x => x.Content);
        }

        public async Task<IEnumerable<T>> Get(string partitionkey, bool shouldBeActive)
        {
            var result = await GetAsync(partitionkey);
            return result.Where(y => y.Active == shouldBeActive).Select(x => x.Content);
        }

        //might be slow. its sorted on rowkey
        public async Task<T> TopElement(string partitionkey)
        {
            var result = await TakeAsync(partitionkey, 1);
            var firstResult = result.FirstOrDefault();

            return firstResult?.Content;
        }

        public new async Task<IEnumerable<T>> Take(int take)
        {
            var result = await TakeAsync(take);
            return result?.Select(x => x.Content);
        }

        #endregion

        #region Post

        public async Task<bool> Post(T element, bool setActive = false)
        {
            var result = await InsertAsync(new JsonStorageEntity<T>(element) { Active = setActive }, true);
            return result.HttpStatusCode == 204;
        }

        public async Task<bool> Post(IEnumerable<T> elements)
        {
            Insert(elements.Select(x => new JsonStorageEntity<T>(x)), true); 
            return await Task.FromResult(true); 
        }

        #endregion

        #region Put

        public async Task<bool> Put(IEnumerable<T> elements)
        {
            Insert(elements.Select(x => new JsonStorageEntity<T>(x)), true, true); 
            return await Task.FromResult(true);
        }

        public async Task<bool> Put(T element, bool setActive = false)
        {
            var result = await InsertAsync(new JsonStorageEntity<T>(element) { Active = setActive }, true, true);
            return result.HttpStatusCode == 204;
        }

        #endregion

        #region Delete

        public async Task<bool> Delete(T element)
        {
            return await Delete(new List<T> { element });
        }

        public async Task<bool> Delete(IEnumerable<T> elements)
        {
            var tasks = elements
                .Select(async (x) => await GetAsync(x.GetPartitionKey(), x.GetRowKey()));

            var all = await Task.WhenAll(tasks);

            DeleteMany(all);

            return true;
        }

        public async Task<bool> Delete(string partitionKey, string rowKey)
        {
            var tag = await GetAsync(partitionKey, rowKey);
            DeleteMany(new[] {tag});

            return true;
        }

        

        #endregion
    }
}
