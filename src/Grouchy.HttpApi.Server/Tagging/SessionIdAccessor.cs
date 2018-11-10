using System;
using Grouchy.Abstractions.Tagging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Grouchy.HttpApi.Server.Tagging
{
   /// <remarks>
   /// Designed to be registered "AsScoped" due to the use of HttpContext
   /// </remarks>>
   public class SessionIdAccessor : ISessionIdAccessor
   {
      private const string SessionIdKey = "session-id";

      private readonly HttpContext _httpContext;

      private string _sessionId;

      public SessionIdAccessor(HttpContext httpContext)
      {
         _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
      }

      public string SessionId
      {
         get
         {
            // Use the cached value if available
            if (_sessionId != null)
            {
               return _sessionId;
            }

            // Otherwise check the headers...
            StringValues idFromHeaders;
            if (_httpContext.Request.Headers.TryGetValue(SessionIdKey, out idFromHeaders))
            {
               _sessionId = idFromHeaders[0];
            }
            else
            {
               _sessionId = "(Not available)";
            }

            return _sessionId;
         }
      }
   }
}
