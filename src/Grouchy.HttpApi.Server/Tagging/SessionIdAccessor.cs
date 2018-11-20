using System;
using Grouchy.Abstractions.Tagging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Grouchy.HttpApi.Server.Tagging
{
   /// <remarks>
   /// Designed to be registered "AsScoped" due to the caching of _sessionId
   /// </remarks>>
   public class SessionIdAccessor : ISessionIdAccessor
   {
      private const string SessionIdKey = "session-id";

      private readonly IHttpContextAccessor _httpContextAccessor;

      private string _sessionId;

      public SessionIdAccessor(IHttpContextAccessor httpContextAccessor)
      {
         _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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

            var httpContext = _httpContextAccessor.HttpContext;

            // Otherwise check the headers...
            if (httpContext != null && httpContext.Request.Headers.TryGetValue(SessionIdKey, out var idFromHeaders))
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
