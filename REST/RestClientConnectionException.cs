using System;

namespace Shared.Common.REST
{
    public class RestClientConnectionException : Exception
    {
        public RestClientConnectionException(string message) : base(message) { }
    }
}
