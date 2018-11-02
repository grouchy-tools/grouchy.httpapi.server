namespace Grouchy.HttpApi.Server.Tests
{
   using System;
   using System.IO;
   using System.Threading;
   using System.Threading.Tasks;
   using Microsoft.AspNetCore.Http;

   public class StubHttpRequest : HttpRequest
   {
      public StubHttpRequest(IHeaderDictionary headers = null)
      {
         Headers = headers;
      }

      public override HttpContext HttpContext { get; }
      public override string Method { get; set; }
      public override string Scheme { get; set; }
      public override bool IsHttps { get; set; }
      public override HostString Host { get; set; }
      public override PathString PathBase { get; set; }
      public override PathString Path { get; set; }
      public override QueryString QueryString { get; set; }
      public override IQueryCollection Query { get; set; }
      public override string Protocol { get; set; }
      public override IHeaderDictionary Headers { get; }
      public override IRequestCookieCollection Cookies { get; set; }
      public override long? ContentLength { get; set; }
      public override string ContentType { get; set; }
      public override Stream Body { get; set; }
      public override bool HasFormContentType { get; }
      public override IFormCollection Form { get; set; }

      public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken())
      {
         throw new NotImplementedException();
      }
   }

   public class StubHttpResponse : HttpResponse
   {
      public override HttpContext HttpContext { get; }
      public override int StatusCode { get; set; }
      public override IHeaderDictionary Headers { get; }
      public override Stream Body { get; set; }
      public override long? ContentLength { get; set; }
      public override string ContentType { get; set; }
      public override IResponseCookies Cookies { get; }
      public override bool HasStarted { get; }

      public override void OnStarting(Func<object, Task> callback, object state)
      {
         throw new NotImplementedException();
      }

      public override void OnCompleted(Func<object, Task> callback, object state)
      {
         throw new NotImplementedException();
      }

      public override void Redirect(string location, bool permanent)
      {
         throw new NotImplementedException();
      }
   }
}
