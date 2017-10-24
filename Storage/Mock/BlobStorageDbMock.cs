using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Shared.Common.Model;

namespace Shared.Common.Storage.Mock
{
    public class BlobStorageDbMock : IBlobStorageDb
    {
        public Func<string> FileAsStringToReturn { get; set; }

        public string GetAsString(string id)
        {
            return FileAsStringToReturn();
        }

        public bool Exists(string blobname)
        {
            return true;
        }

        public string UploadFile(byte[] file, string name)
        {
            throw new NotImplementedException();
        }

        public string UploadFile(Stream file, string name, string contentType)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UploadFileAsync(Stream file, string name, string contentType)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UploadStringAsync(string file, string name, string contentType)
        {
            throw new NotImplementedException();
        }

        public Stream GetStream(string id)
        {
            throw new NotImplementedException();
        }

        public byte[] GetBytes(string id)
        {
            throw new NotImplementedException();
        }

        

        public async Task<Stream> ReadStream(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void ThrowIfNotExists(string id, int lengthGreaterThan = 1)
        {
            throw new NotImplementedException();
        }

        public void RemoveBlob(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BlobModel> ListAllBlobs()
        {
            throw new NotImplementedException();
        }
    }
}
