using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Shared.Common.REST
{
    // ReSharper disable once RedundantExtendsListEntry
    public class Oauth2Client : JsonRestClient, ISimpleRestClient
    {
        public string StsEndpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Scope { get; set; }

        public Oauth2Client(Oauth2Parameters args)
            : this(args.StsEndpoint, args.ClientId, args.ClientSecret, args.Username, args.Password, args.Scope)
        {
        }

        public Oauth2Client(string stsEndpoint, string clientId, string clientSecret, string username, string password, string scope)
        {
            StsEndpoint = stsEndpoint;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Username = username;
            Password = password;
            Scope = scope;
        }

        public override async Task<TReturnType> Get<TReturnType>(string url)
        {
            return await Get<TReturnType>(url, true);
        }

        public async Task<TReturnType> Get<TReturnType>(string url, bool firstTry)
        {
            try
            {
                return await base.Get<TReturnType>(url);
            }
            catch (HttpRequestException r)
            {
                if (!IsAccessTokenOld(r))
                    throw;

                if(!firstTry) 
                    throw new Exception("While getting " + url + ": refreshed token but still getting error", r);

                lock (_requireLock)
                {
                    _accessTokenOld = true;
                }

                return await Get<TReturnType>(url, false);
            }
        }

        public override async Task<TReturnType> Post<TContentType, TReturnType>(string url, TContentType content)
        {
            try
            {
                return await base.Post<TContentType, TReturnType>(url, content);
            }
            catch (HttpRequestException r)
            {
                if (!IsAccessTokenOld(r))
                    throw;

                lock (_requireLock)
                {
                    _accessTokenOld = true;
                }

                return await Post<TContentType, TReturnType>(url, content);
            }
        }

        private bool IsAccessTokenOld(HttpRequestException r)
        {
            var responseMessage = (HttpResponseMessageException) r.InnerException;
            // ReSharper disable once PossibleNullReferenceException
            return responseMessage.ResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized;
        }

        protected override HttpClient GetClient()
        {
            var token = GetAccessToken();

            var client = new HttpClient()
            {
                Timeout = new TimeSpan(0, 30, 0)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        private bool _accessTokenOld;
        private readonly object _requireLock = new object();
        private string GetAccessToken()
        {
            if(_accessToken == null || _accessTokenOld)
            {
                lock(_requireLock)
                {
                    if (_accessToken == null || _accessTokenOld)
                    {
                        // ReSharper disable once UnusedVariable
                        var success = SetNewAccessToken().Result;
                    }
                }
            }

            return _accessToken;
        }

        private string _accessToken;
        private TokenResponse _tokenResponse;
        private async Task<bool> SetNewAccessToken()
        {
            var oauth2Client = new TokenClient(StsEndpoint, ClientId, ClientSecret);

            _tokenResponse = await oauth2Client.RequestResourceOwnerPasswordAsync(Username, Password, Scope);
            if (string.IsNullOrEmpty(_tokenResponse.AccessToken))
                throw new RestClientConnectionException($"Didnt get an accesstoken from endpoint " + StsEndpoint + " using username " + Username);

            _accessToken = _tokenResponse.AccessToken;

            return true;
        }
    }
}
