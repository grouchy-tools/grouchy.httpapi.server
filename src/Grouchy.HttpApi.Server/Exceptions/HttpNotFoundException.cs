using System.Net;

namespace Grouchy.HttpApi.Server.Exceptions
{
   public class HttpNotFoundException : HttpException
   {
      public HttpNotFoundException(string message = null) : base(HttpStatusCode.NotFound, message ?? "Not found")
      {
      }
   }
}