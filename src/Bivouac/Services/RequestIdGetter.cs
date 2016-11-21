namespace Bivouac.Services
{
   using System;
   using Bivouac.Abstractions;
   using Microsoft.AspNetCore.Http;
   using Microsoft.Extensions.Primitives;

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
         if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
         if (guidGenerator == null) throw new ArgumentNullException(nameof(guidGenerator));

         _httpContext = httpContext;
         _guidGenerator = guidGenerator;
      }

      public string Get()
      {
         // Use the cached value if available
         if (_requestId != null)
         {
            return _requestId;
         }

         // Otherwise check the headers...
         StringValues idFromHeaders;
         if (_httpContext.Request.Headers.TryGetValue(RequestIdKey, out idFromHeaders))
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
