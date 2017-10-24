using Microsoft.WindowsAzure.Storage;

namespace Shared.AzureCommon
{
    public abstract class BaseAzureStorage
    {
        private readonly string _connectionString;

        public BaseAzureStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private CloudStorageAccount _storageAccount;
        protected CloudStorageAccount StorageAccount
        {
            get
            {
                if (_storageAccount == null)
                    _storageAccount = CloudStorageAccount.Parse(_connectionString);

                return _storageAccount;
            }
        }

        protected void Dismiss()
        {
            _storageAccount = null;
        }
    }
}
