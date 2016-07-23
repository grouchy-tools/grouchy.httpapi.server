namespace Bivouac.Exceptions
{
   using System.Net;

   public class HttpNotFoundException : HttpException
   {
      public HttpNotFoundException(string message = null) : base(HttpStatusCode.NotFound, message ?? "Not found")
      {
      }
   }
}