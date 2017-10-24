using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Common.Storage.Mock
{
    public class JsonStorageDictionaryDb<T> : IJsonStorage<T>, ISyncJsonStorage<T> where T : class, IJsonStorageEntity 
    {
        private readonly Dictionary<string, Dictionary<string, T>> _db;

        public JsonStorageDictionaryDb()
        {
            _db = new Dictionary<string, Dictionary<string, T>>();
        }

        public T Get(string partitionkey, string rowkey, bool throwExceptionIfNotFound)
        {
            if ((!_db.ContainsKey(partitionkey) || !_db[partitionkey].ContainsKey(rowkey)) && !throwExceptionIfNotFound)
                return default(T);

            return _db[partitionkey][rowkey];
        }

        public async Task<TSuper> GetSuper<TSuper>(string partitionkey, string rowkey, bool throwExceptionIfNotFound = true) where TSuper : T
        {
            return (TSuper) Convert.ChangeType(Get(partitionkey, rowkey, throwExceptionIfNotFound), typeof(TSuper));
;        }

        public IEnumerable<T> Get()
        {
            if (!_db.Any())
            {
                return new List<T>();
            }
            return _db.Values.SelectMany(x => x.Values);
        }

        public IEnumerable<T> Get(string partitionkey)
        {
            if(!_db.ContainsKey(partitionkey))
                return new List<T>();

            return _db[partitionkey].Values;
        }

        public IEnumerable<T> Take(int take)
        {
            if (!_db.Any())
            {
                return new List<T>();
            }
            return _db.Values.SelectMany(x => x.Values).Take(take);
        }

        public bool Post(T element, bool setActive = false)
        {
            if(!_db.ContainsKey(element.GetPartitionKey()))
                _db.Add(element.GetPartitionKey(), new Dictionary<string, T>());

            if (_db[element.GetPartitionKey()].ContainsKey(element.GetRowKey()))
                return true;

            _db[element.GetPartitionKey()].Add(element.GetRowKey(), element);
            return true;
        }

        public bool Put(T element, bool setActive = false)
        {
            if (!_db.ContainsKey(element.GetPartitionKey()))
                _db.Add(element.GetPartitionKey(), new Dictionary<string, T>());

            if (_db[element.GetPartitionKey()].ContainsKey(element.GetRowKey()))
                _db[element.GetPartitionKey()][element.GetRowKey()] = element;
            else
                _db[element.GetPartitionKey()].Add(element.GetRowKey(), element);

            return true;
        }

        public bool Delete(T element)
        {
            if (!_db.ContainsKey(element.GetPartitionKey()))
                return false;

            if (!_db[element.GetPartitionKey()].ContainsKey(element.GetRowKey()))
                return false;
            
            _db[element.GetPartitionKey()].Remove(element.GetRowKey());
            return true;
        }

        public bool Delete(string partitionKey, string rowKey)
        {
            if (!_db.ContainsKey(partitionKey))
                return false;
            if (!_db[partitionKey].ContainsKey(rowKey))
                return false;
            _db[partitionKey].Remove(rowKey);
            return true;
        }

        public Task<T> TopElement(string partitionkey)
        {
            if (!_db.ContainsKey(partitionkey))
                return Task.FromResult(default(T));

            return Task.FromResult(_db[partitionkey].Values.FirstOrDefault());
        }


        Task<IEnumerable<T>> IJsonStorage<T>.Get(string partitionkey)
        {
            return Task.FromResult(Get(partitionkey));
        }

        public Task<IEnumerable<T>> Get(string partitionkey, bool shouldBeActive)
        {
            return Task.FromResult(Get(partitionkey));
        }

        Task<bool> IJsonStorage<T>.Post(T element, bool setActive)
        {
            return Task.FromResult(Post(element));
        }

        public Task<bool> Post(IEnumerable<T> elements)
        {
            foreach (var element in elements)
                Post(element);

            return Task.FromResult<bool>(true);
        }

        Task<bool> IJsonStorage<T>.Put(T element, bool setActive)
        {
            return Task.FromResult(Put(element));
        }

        public Task<bool> Put(IEnumerable<T> elements)
        {
            foreach (var element in elements)
                Put(element);

            return Task.FromResult<bool>(true);
        }

        Task<bool> IJsonStorage<T>.Delete(T element)
        {
            return Task.FromResult(Delete(element));
        }

        public Task<bool> Delete(IEnumerable<T> elements)
        {
            foreach (var element in elements)
                Delete(element);

            return Task.FromResult<bool>(true);
        }

        Task<bool> IJsonStorage<T>.Delete(string partitionKey, string rowKey)
        {
            return Task.FromResult(Delete(partitionKey, rowKey));
        }

        Task<T> IJsonStorage<T>.Get(string partitionkey, string rowkey, bool throwExceptionIfNotFound)
        {
            return Task.FromResult(Get(partitionkey, rowkey, throwExceptionIfNotFound));
        }

        Task<IEnumerable<T>> IJsonStorage<T>.Get()
        {
            return Task.FromResult(Get());
        }

        Task<IEnumerable<T>> IJsonStorage<T>.Take(int take)
        {
            return Task.FromResult(Take(take));
        }

      
    }
}
