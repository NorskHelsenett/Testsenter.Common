using System.Collections.Generic;
using System.Linq;
using Shared.Common.Storage;

namespace Shared.AzureCommon.TableStorage.JsonStorage
{
    public class SyncJsonTableStorageDb<T> : TableStorageDb<JsonStorageEntity<T>>, ISyncJsonStorage<T> where T : class, IJsonStorageEntity
    {
        public SyncJsonTableStorageDb(string connectionString, string tableName) : base(connectionString, tableName) { }

        #region Get

        public new T Get(string partitionkey, string rowkey, bool throwExceptionIfNotFound = true)
        {
            return base.Get(partitionkey, rowkey, throwExceptionIfNotFound)?.Content;
        }

        public new IEnumerable<T> Get(string partitionkey)
        {
            var result = base.Get(partitionkey);
            return result.Select(x => x.Content);
        }

        #endregion

        #region Post

        public bool Post(T element, bool setActive = false)
        {
            var result = Insert(new JsonStorageEntity<T>(element) { Active = setActive }, true);
            return result.HttpStatusCode == 204;
        }

        #endregion

        #region Put

        public bool Put(T element, bool setActive = false)
        {
            var result = Insert(new JsonStorageEntity<T>(element) { Active = setActive }, true, true);
            return result.HttpStatusCode == 204;
        }

        public bool Delete(T element)
        {
            var x = new[] { new JsonStorageEntity<T>(element) };
            DeleteMany(x);

            return true;
        }

        #endregion
    }
}
