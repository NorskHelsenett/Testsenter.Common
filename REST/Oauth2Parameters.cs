namespace Shared.Common.REST
{
    public class Oauth2Parameters
    {
        public Oauth2Parameters() { }

        public Oauth2Parameters(string stsEndpoint, string clientId, string clientSecret, string username, string password, string scope)
        {
            StsEndpoint = stsEndpoint;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Username = username;
            Password = password;
            Scope = scope;
        }

        public string StsEndpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Scope { get; set; }
    }
}
