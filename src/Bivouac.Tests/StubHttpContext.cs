namespace Bivouac.Tests
{
   using System;
   using System.Collections.Generic;
   using System.Security.Claims;
   using System.Threading;
   using Microsoft.AspNetCore.Http;
   using Microsoft.AspNetCore.Http.Authentication;
   using Microsoft.AspNetCore.Http.Features;

   public class StubHttpContext : HttpContext
   {
      public StubHttpContext(HttpRequest request, HttpResponse response)
      {
         Request = request;
         Response = response;
      }

      public override IFeatureCollection Features { get; }
      public override HttpRequest Request { get; }
      public override HttpResponse Response { get; }
      public override ConnectionInfo Connection { get; }
      public override WebSocketManager WebSockets { get; }
      [Obsolete]
      public override AuthenticationManager Authentication { get; }
      public override ClaimsPrincipal User { get; set; }
      public override IDictionary<object, object> Items { get; set; }
      public override IServiceProvider RequestServices { get; set; }
      public override CancellationToken RequestAborted { get; set; }
      public override string TraceIdentifier { get; set; }
      public override ISession Session { get; set; }

      public override void Abort()
      {
         throw new NotImplementedException();
      }
   }
}