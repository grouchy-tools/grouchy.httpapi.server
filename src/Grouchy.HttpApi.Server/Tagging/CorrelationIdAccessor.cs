using System;
using Grouchy.Abstractions;
using Grouchy.Abstractions.Tagging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Grouchy.HttpApi.Server.Tagging
{
   /// <remarks>
   /// Designed to be registered "AsScoped" due to the use of HttpContext
   /// </remarks>>
   public class CorrelationIdAccessor : ICorrelationIdAccessor
   {
      private const string CorrelationIdKey = "correlation-id";

      private readonly HttpContext _httpContext;
      private readonly IGenerateGuids _guidGenerator;

      private string _correlationId;

      public CorrelationIdAccessor(
         HttpContext httpContext,
         IGenerateGuids guidGenerator)
      {
         _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
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

            // Otherwise check the headers...
            StringValues idFromHeaders;
            if (_httpContext.Request.Headers.TryGetValue(CorrelationIdKey, out idFromHeaders))
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
