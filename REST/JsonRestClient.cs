using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;

namespace Shared.Common.REST
{
    public class JsonRestClient : ISimpleRestClient
    {
        private TimeSpan _timeout = new TimeSpan(1, 0, 0);
        public int TimeoutInMinutes
        {
            get
            {
                return _timeout.Minutes;
            }
            set
            {
                _timeout = new TimeSpan(0, value, 0);
            } 
        }

        public virtual async Task<TReturnType> Delete<TReturnType>(string url)
        {
            using (var client = GetClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TReturnType>(result);
                }

                throw new HttpRequestException("Did not receive sucessfull statuscode while deleteing " + url + ". Response: " + JsonConvert.SerializeObject(response), new HttpResponseMessageException(response));
            }
        }

        public virtual async Task<TReturnType> Get<TReturnType>(string url)
        {
            using (var client = GetClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    try
                    {
                        return JsonConvert.DeserializeObject<TReturnType>(result);
                    }
                    catch (JsonSerializationException e)
                    {
                        throw new Exception("While requesting url " + url + ", could not deserialize answer " + result, e);
                    }
                }

                throw new HttpRequestException("Did not receive sucessfull statuscode while getting " + url + ". Response: " + JsonConvert.SerializeObject(response), new HttpResponseMessageException(response));
            }
        }

        public virtual async Task<TReturnType> Post<TContentType, TReturnType>(string url, TContentType content)
        {
            using (var client = GetClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TReturnType>(result);
                }

                throw new HttpRequestException("Did not receive sucessfull statuscode while posting " + url + ". Response: " + JsonConvert.SerializeObject(response), new HttpResponseMessageException(response));
            }
        }

        protected virtual HttpClient GetClient()
        {
            var client = new HttpClient()
            {
                Timeout = _timeout
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
