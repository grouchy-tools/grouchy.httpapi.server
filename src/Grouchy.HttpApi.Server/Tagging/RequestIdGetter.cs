using System;
using Grouchy.Abstractions;
using Grouchy.HttpApi.Server.Abstractions.Tagging;
using Microsoft.AspNetCore.Http;

namespace Grouchy.HttpApi.Server.Tagging
{
   /// <remarks>
   /// Designed to be registered "AsScoped" due to the caching of _requestId
   /// </remarks>>
   public class InboundRequestIdGetter : IInboundRequestIdAccessor
   {
      private const string RequestIdKey = "request-id";

      private readonly IHttpContextAccessor _httpContextAccessor;
      private readonly IGenerateGuids _guidGenerator;

      private string _requestId;

      public InboundRequestIdGetter(
         IHttpContextAccessor httpContextAccessor,
         IGenerateGuids guidGenerator)
      {
         _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
         _guidGenerator = guidGenerator ?? throw new ArgumentNullException(nameof(guidGenerator));
      }

      public string InboundRequestId
      {
         get
         {
            // Use the cached value if available
            if (_requestId != null)
            {
               return _requestId;
            }

            var httpContext = _httpContextAccessor.HttpContext;

            // Otherwise check the headers...
            if (httpContext != null && httpContext.Request.Headers.TryGetValue(RequestIdKey, out var idFromHeaders))
            {
               _requestId = idFromHeaders[0];
            }
            else
            {
               // Create one if not
               _requestId = _guidGenerator.Generate().ToString();
            }

            return _requestId;
         }
      }
   }
}
