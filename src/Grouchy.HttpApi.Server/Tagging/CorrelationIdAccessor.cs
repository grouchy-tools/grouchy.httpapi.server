using System;
using Grouchy.Abstractions;
using Grouchy.Abstractions.Tagging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Grouchy.HttpApi.Server.Tagging
{
   /// <remarks>
   /// Designed to be registered "AsScoped" due to the caching of _correlationId
   /// </remarks>>
   public class CorrelationIdAccessor : ICorrelationIdAccessor
   {
      private const string CorrelationIdKey = "correlation-id";

      private readonly IHttpContextAccessor _httpContextAccessor;
      private readonly IGenerateGuids _guidGenerator;

      private string _correlationId;

      public CorrelationIdAccessor(
         IHttpContextAccessor httpContextAccessor,
         IGenerateGuids guidGenerator)
      {
         _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
         _guidGenerator = guidGenerator ?? throw new ArgumentNullException(nameof(guidGenerator));
      }

      public string CorrelationId
      {
         get
         {
            // Use the cached value if available
            if (_correlationId != null)
            {
               return _correlationId;
            }

            var httpContext = _httpContextAccessor.HttpContext;

            // Otherwise check the headers...
            if (httpContext != null && httpContext.Request.Headers.TryGetValue(CorrelationIdKey, out var idFromHeaders))
            {
               _correlationId = idFromHeaders[0];
            }
            else
            {
               // Create one if not
               _correlationId = _guidGenerator.Generate().ToString();
            }

            return _correlationId;
         }
      }
   }
}
