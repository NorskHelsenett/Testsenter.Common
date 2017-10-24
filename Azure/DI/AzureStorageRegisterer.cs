using Shared.Common.DI;
using Shared.Common.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Shared.AzureCommon.BlobStorage;
using Shared.AzureCommon.Queue;
using Shared.AzureCommon.TableStorage;
using Shared.AzureCommon.TableStorage.JsonStorage;

namespace Shared.AzureCommon.DI
{
    public class AzureStorageRegisterer : IRegisterStorage
    {
        private readonly UnityDependencyInjector _di;

        public AzureStorageRegisterer(UnityDependencyInjector di)
        {
            _di = di;
        }

        public void RegisterJsonTableStorage<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : class, IJsonStorageEntity
        {
            _di.Register<IJsonStorage<TType>, JsonTableStorageDb<TType>>(lifeTime, new object[] { storageAccountSettingsvalue, tableName });
        }

        public void RegisterJsonTableStorageWithTypeName<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : class, IJsonStorageEntity
        {
            _di.Register<IJsonStorage<TType>, JsonTableStorageDb<TType>>(lifeTime, tableName, new object[] { storageAccountSettingsvalue, tableName });
        }

        public void RegisterSyncJsonTableStorage<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : class, IJsonStorageEntity
        {
            _di.Register<ISyncJsonStorage<TType>, SyncJsonTableStorageDb<TType>>(lifeTime, new object[] { storageAccountSettingsvalue, tableName });
        }

        public void RegisterSyncJsonTableStorageWithTypeName<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : class, IJsonStorageEntity
        {
            _di.Register<ISyncJsonStorage<TType>, SyncJsonTableStorageDb<TType>>(lifeTime, tableName, new object[] { storageAccountSettingsvalue, tableName });
        }

        public void RegisterBlobStorage(InstanceLifetime lifeTime, string blobContainerName, string storageAccountSettingsvalue)
        {
            _di.Register<IBlobStorageDb, BlobStorageDb>(lifeTime, blobContainerName, new object[] { storageAccountSettingsvalue, blobContainerName });
        }

        public void RegisterQueue(InstanceLifetime lifeTime, string queueName, string storageAccountSettingsvalue)
        {
            _di.Register<IQueue, AzureQueue>(lifeTime, queueName.ToString(), new object[] { storageAccountSettingsvalue, queueName });
        }

        public void RegisterTableStorageDb<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : TableEntity, new()
        {
            _di.Register<ITableStorageDb<TType>, TableStorageDb<TType>>(lifeTime, new object[] { storageAccountSettingsvalue, tableName });
        }

        public ITableStorageDb<TType> GetTableStorageDb<TType>(string tableName, string storageAccountSettingsvalue) where TType : TableEntity, new()
        {
            return new TableStorageDb<TType>(storageAccountSettingsvalue, tableName);
        }
    }
}
