namespace Bivouac.Exceptions
{
   using System;
   using System.Net;

   public class HttpException : Exception
   {
      public HttpException(HttpStatusCode statusCode, string message = null) : base(message)
      {
         StatusCode = statusCode;
      }

      public HttpStatusCode StatusCode { get; private set; }
   }
}