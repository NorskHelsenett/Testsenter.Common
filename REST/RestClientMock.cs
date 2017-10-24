using System;
using System.Threading.Tasks;

namespace Shared.Common.REST
{
    public class RestClientMock : ISimpleRestClient
    {
        public object ReturnThis { get; set; }

        public Task<TReturnType> Delete<TReturnType>(string url)
        {
            throw new NotImplementedException();
        }

        public Task<TReturnType> Get<TReturnType>(string url)
        {
            return Task.Run(() =>
            {
                return (TReturnType)Convert.ChangeType(ReturnThis, typeof(TReturnType));
            });
        }

        public Task<TReturnType> Post<TContentType, TReturnType>(string url, TContentType content)
        {
            throw new NotImplementedException();
        }
    }
}
