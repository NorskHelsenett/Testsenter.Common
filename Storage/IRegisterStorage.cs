using Microsoft.WindowsAzure.Storage.Table;
using Shared.Common.DI;

namespace Shared.Common.Storage
{
    public interface IRegisterStorage
    {
        //void RegisterJsonTableStorage<TType>(InstanceLifetime lifeTime, Constants.TableNames tableName, string storageAccountSettingsKey = "AllCommonAzureStorageAccountForEnvironment") where TType : class, IJsonStorageEntity;
        //void RegisterJsonTableStorageWithTypeName<TType>(InstanceLifetime lifeTime, Constants.TableNames tableName, string storageAccountSettingsKey = "AllCommonAzureStorageAccountForEnvironment") where TType : class, IJsonStorageEntity;
        //void RegisterSyncJsonTableStorage<TType>(InstanceLifetime lifeTime, Constants.TableNames tableName, string storageAccountSettingsKey = "AllCommonAzureStorageAccountForEnvironment") where TType : class, IJsonStorageEntity;
        //void RegisterSyncJsonTableStorageWithTypeName<TType>(InstanceLifetime lifeTime, Constants.TableNames tableName, string storageAccountSettingsKey = "AllCommonAzureStorageAccountForEnvironment") where TType : class, IJsonStorageEntity;
        //void RegisterBlobStorage(InstanceLifetime lifeTime, Constants.BlobContainers blobContainerName, string storageAccountSettingsKey = Configurationsetting.Common.AzureStorageAccountForEnvironment);
        //void RegisterQueue(InstanceLifetime lifeTime, Constants.QueueNames queueName, string storageAccountSettingsKey = Configurationsetting.Common.AzureStorageAccountForEnvironment);
        //void RegisterTableStorageDb<TType>(InstanceLifetime lifeTime, Constants.TableNames tableName, string storageAccountSettingsKey = Configurationsetting.Common.AzureStorageAccountForEnvironment) where TType : TableEntity, new();
        //ITableStorageDb<TType> GetTableStorageDb<TType>(Constants.TableNames tableName, string storageAccountSettingsKey = Configurationsetting.Common.AzureStorageAccountForEnvironment) where TType : TableEntity, new();

        void RegisterJsonTableStorage<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : class, IJsonStorageEntity;
        void RegisterJsonTableStorageWithTypeName<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : class, IJsonStorageEntity;
        void RegisterSyncJsonTableStorage<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : class, IJsonStorageEntity;
        void RegisterSyncJsonTableStorageWithTypeName<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : class, IJsonStorageEntity;
        void RegisterBlobStorage(InstanceLifetime lifeTime, string blobContainerName, string storageAccountSettingsvalue);
        void RegisterQueue(InstanceLifetime lifeTime, string queueName, string storageAccountSettingsvalue);
        void RegisterTableStorageDb<TType>(InstanceLifetime lifeTime, string tableName, string storageAccountSettingsvalue) where TType : TableEntity, new();
        ITableStorageDb<TType> GetTableStorageDb<TType>(string tableName, string storageAccountSettingsvalue) where TType : TableEntity, new();
    }
}
