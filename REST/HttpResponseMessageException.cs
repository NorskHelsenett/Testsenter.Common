using System;
using System.Net.Http;

namespace Shared.Common.REST
{
    public class HttpResponseMessageException : Exception
    {
        public HttpResponseMessage ResponseMessage { get; set; }
        public HttpResponseMessageException(HttpResponseMessage hrm)
        {
            ResponseMessage = hrm;
        }
    }
}
