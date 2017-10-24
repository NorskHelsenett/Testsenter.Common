
using System.Threading.Tasks;

namespace Shared.Common.REST
{
    public interface ISimpleRestClient
    {
        Task<TReturnType> Delete<TReturnType>(string url);
        Task<TReturnType> Get<TReturnType>(string url);
        Task<TReturnType> Post<TContentType, TReturnType>(string url, TContentType content);
    }
}
