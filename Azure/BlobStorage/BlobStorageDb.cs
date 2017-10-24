using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Shared.Common.Logic;
using Shared.Common.Model;
using Shared.Common.Storage;

namespace Shared.AzureCommon.BlobStorage
{
    public class BlobStorageDb : BaseAzureStorage, IBlobStorageDb
    {
        private CloudBlobClient _client;
        protected CloudBlobClient Client
        {
            get
            {
                if (_client == null)
                    _client = StorageAccount.CreateCloudBlobClient();

                return _client;
            }
        }

        private readonly string _containerName;

        public BlobStorageDb(string connectionString, Enum name)
            : this(connectionString, name.ToString())
        {
        }

        public BlobStorageDb(string connectionString, string containerName)
            : base(connectionString)
        {
            _containerName = containerName.ToString().ToLower();
        }

        protected static string GetSasToken(CloudBlobContainer container)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(60);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List;

            return container.GetSharedAccessSignature(sasConstraints);
        }

        public IEnumerable<BlobModel> ListAllBlobs()
        {
            var list = Client.GetContainerReference(_containerName)
                                .ListBlobs(useFlatBlobListing: true)
                                .OfType<CloudBlob>()
                                .OrderByDescending(b => b.Properties.LastModified);
            var listOfFileNames = new List<BlobModel>();

            foreach (var blob in list)
            {
                listOfFileNames.Add(new BlobModel
                {
                    Name = blob.Name,
                    Url = blob.Uri.AbsoluteUri,
                    ContentType = blob.Properties.ContentType
                });
            }

            return listOfFileNames;
        }

        public bool Exists(string blobname)
        {
            try
            {
                ThrowIfNotExists(blobname, 10);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string UploadFile(byte[] file, string name)
        {
            return UploadFile(new MemoryStream(file), name, null);
        }

        public void ThrowIfNotExists(string id, int lengthGreaterThan = 1)
        {
            var blob = GetBlob(id);

            if (blob == null)
                throw new ArgumentException("Could not find blob with id " + id);

            blob.FetchAttributes();

            if (blob.Properties.Length <= lengthGreaterThan)
                throw new ArgumentException("Found blob, but length: " + blob.Properties.Length + " is less than required length: " + lengthGreaterThan);
        }

        public void RemoveBlob(string id)
        {
            var blob = GetBlob(id);
            blob.DeleteIfExists();
        }

        public string UploadFile(Stream file, string name, string contentType)
        {
            var container = Client.GetContainerReference(_containerName);
            container.CreateIfNotExists();

            var blob = container.GetBlockBlobReference(name);
            blob.UploadFromStream(file);

            if (!string.IsNullOrEmpty(contentType))
            {
                blob.Properties.ContentType = contentType;
                blob.SetProperties();
            }

            return container.Uri + "/" + name + GetSasToken(container);
        }

        public async Task<string> UploadFileAsync(Stream file, string name, string contentType)
        {
            var container = Client.GetContainerReference(_containerName);
            container.CreateIfNotExists();

            var blob = container.GetBlockBlobReference(name);
            await blob.UploadFromStreamAsync(file);

            if (!string.IsNullOrEmpty(contentType))
            {
                blob.Properties.ContentType = contentType;
                blob.SetProperties();
            }

            return container.Uri + "/" + name + GetSasToken(container);
        }

        public async Task<string> UploadStringAsync(string file, string name, string contentType)
        {
            var container = Client.GetContainerReference(_containerName);
            container.CreateIfNotExists();

            var blob = container.GetBlockBlobReference(name);

            using (var stream = DataHelper.GenerateStreamFromString(file))
            {
                await blob.UploadFromStreamAsync(stream);
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                blob.Properties.ContentType = contentType;
                blob.SetProperties();
            }

            return container.Uri + "/" + name + GetSasToken(container);
        }

        public Stream GetStream(string id)
        {
            var blob = GetBlob(id);
            var result = new MemoryStream();
            blob.DownloadToStream(result);

            result.Position = 0;
            return result;
        }

        public async Task<Stream> ReadStream(string id, CancellationToken cancellationToken)
        {
            var blob = GetBlob(id);
            return await blob.OpenReadAsync(cancellationToken);
        }

        public byte[] GetBytes(string id)
        {
            using (var memoryStream = new MemoryStream())
            {
                GetStream(id).CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private CloudBlockBlob GetBlob(string id)
        {
            var container = Client.GetContainerReference(_containerName);
            container.CreateIfNotExists();

            return container.GetBlockBlobReference(id);
        }

        public string GetAsString(string id)
        {
            return System.Text.Encoding.UTF8.GetString(GetBytes(id));
        }

        public static string RemoveSasLink(string s)
        {
            try
            {
                return s.Split('?')[0];
            }
            catch (Exception)
            {
                return s;
            }
        }

        public void Dispose()
        {
            _client = null;
            base.Dismiss();
        }
    }
}
