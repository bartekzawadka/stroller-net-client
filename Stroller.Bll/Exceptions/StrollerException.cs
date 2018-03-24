using System;
using System.Net;

namespace Stroller.Bll.Exceptions
{
    public class StrollerException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public StrollerException(HttpStatusCode code, string message) : base(message)
        {
            StatusCode = code;
        }

        public StrollerException(HttpStatusCode code, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = code;
        }
    }
}
