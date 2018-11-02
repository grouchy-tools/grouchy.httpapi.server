using System;
using System.Net;

namespace Grouchy.HttpApi.Server.Exceptions
{
   public class HttpException : Exception
   {
      public HttpException(HttpStatusCode statusCode, string message = null) : base(message)
      {
         StatusCode = statusCode;
      }

      public HttpStatusCode StatusCode { get; private set; }
   }
}