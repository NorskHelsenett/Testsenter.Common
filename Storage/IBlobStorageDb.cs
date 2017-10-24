using Shared.Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Common.Storage
{
    public interface IBlobStorageDb
    {
        bool Exists(string blobname);
        string UploadFile(byte[] file, string name);
        string UploadFile(Stream file, string name, string contentType);
        Task<string> UploadFileAsync(Stream file, string name, string contentType);
        Task<string> UploadStringAsync(string file, string name, string contentType);
        Stream GetStream(string id);
        byte[] GetBytes(string id);
        string GetAsString(string id);
        Task<Stream> ReadStream(string id, CancellationToken cancellationToken);
        void ThrowIfNotExists(string id, int lengthGreaterThan = 1);
        void RemoveBlob(string id);
        IEnumerable<BlobModel> ListAllBlobs();
    }
}
