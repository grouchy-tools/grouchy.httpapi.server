namespace Bivouac.Services
{
   using System;
   using Bivouac.Abstractions;
   using Microsoft.AspNetCore.Http;
   using Microsoft.Extensions.Primitives;

   public class CorrelationIdGetter : IGetCorrelationId
   {
      private const string CorrelationIdKey = "correlation-id";

      private readonly IHttpContextAccessor _httpContextAccessor;
      private readonly IGenerateGuids _guidGenerator;

      public CorrelationIdGetter(
         IHttpContextAccessor httpContextAccessor,
         IGenerateGuids guidGenerator)
      {
         if (httpContextAccessor == null) throw new ArgumentNullException(nameof(httpContextAccessor));
         if (guidGenerator == null) throw new ArgumentNullException(nameof(guidGenerator));

         _httpContextAccessor = httpContextAccessor;
         _guidGenerator = guidGenerator;
      }

      public string Get()
      {
         var context = _httpContextAccessor.HttpContext;

         // Inspect HttpContext first...
         object idFromContext;
         if (context.Items.TryGetValue(CorrelationIdKey, out idFromContext))
         {
            return (string)idFromContext;
         }

         string id;

         // Then headers...
         StringValues idFromHeaders;
         if (context.Request.Headers.TryGetValue(CorrelationIdKey, out idFromHeaders))
         {
            id = idFromHeaders[0];
         }
         else
         {
            // Create one if not in HttpContext or Headers
            id = _guidGenerator.Generate().ToString();
         }

         // Finally add to HttpContext
         context.Items.Add(CorrelationIdKey, id);
         return id;
      }
   }
}
