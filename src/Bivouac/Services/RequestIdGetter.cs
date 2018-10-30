using System;
using Bivouac.Abstractions;
using Burble.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Bivouac.Services
{
   /// <remarks>
   /// Designed to be registered "AsScoped" due to the use of HttpContext
   /// </remarks>>
   public class RequestIdGetter : IGetRequestId
   {
      private const string RequestIdKey = "request-id";

      private readonly HttpContext _httpContext;
      private readonly IGenerateGuids _guidGenerator;

      private string _requestId;

      public RequestIdGetter(
         HttpContext httpContext,
         IGenerateGuids guidGenerator)
      {
         _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
         _guidGenerator = guidGenerator ?? throw new ArgumentNullException(nameof(guidGenerator));
      }

      public string Get()
      {
         // Use the cached value if available
         if (_requestId != null)
         {
            return _requestId;
         }

         // Otherwise check the headers...
         if (_httpContext.Request.Headers.TryGetValue(RequestIdKey, out var idFromHeaders))
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
