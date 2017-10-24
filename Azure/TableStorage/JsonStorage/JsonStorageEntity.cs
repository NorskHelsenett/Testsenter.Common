using System;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Shared.Common.Storage;

namespace Shared.AzureCommon.TableStorage.JsonStorage
{
    public class JsonStorageEntity<T> : TableEntity where T : class, IJsonStorageEntity
    {
        [IgnoreProperty]
        public T Content {
            get
            {
                return JsonConvert.DeserializeObject<T>(ContentAsJson);
            }
            set
            {
                ContentAsJson = JsonConvert.SerializeObject(value);
            }
        }

        public bool Active { get; set; }

        public string ContentAsJson { get; set; }

        [Obsolete("For serialization purposes only")]
        public JsonStorageEntity() { }

        public JsonStorageEntity(T content) :
            base(content.GetPartitionKey(), content.GetRowKey())
        {
            Content = content;
        }
    }
}
