using System.Collections.Generic;

namespace Shared.Common.Logic
{
    public class LockedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly object _lock;

        public LockedDictionary() 
        {
            _lock = new object();
        }

        public new TValue this[TKey key]
        {
            get
            {
                lock (_lock)
                {
                    return base[key];
                }
            }
            set
            {
                lock (_lock)
                {
                    base[key] = value;
                }
            }
        }

        public new void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                if(!ContainsKey(key))
                    base.Add(key, value);
            }
        }

        public new bool ContainsKey(TKey key)
        {
            bool result;
            lock (_lock)
            {
                result = base.ContainsKey(key);
            }
            return result;
        }
    }
}
