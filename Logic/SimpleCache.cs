using System;

namespace Shared.Common.Logic
{
    public class SimpleCache
    {
        private readonly int _timeoutInMinutes;
        DateTime _cacheExpiry;

        public SimpleCache(int timeoutInMinutes)
        {
            _timeoutInMinutes = timeoutInMinutes;
            SetNextCache();
        }

        public bool CheckTimeOut()
        {
            return DateTime.Now.CompareTo(_cacheExpiry) > 0;
        }

        public bool TimeOut()
        {
            if (CheckTimeOut())
            {
                SetNextCache();
                return true;
            }

            return false;
        }

        public void Flush()
        {
            SetNextCache();
        }

        private void SetNextCache()
        {
            _cacheExpiry = DateTime.Now.AddMinutes(_timeoutInMinutes);
        }
    }
}
