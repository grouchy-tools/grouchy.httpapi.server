﻿namespace Bivouac.Implementations
{
   using System;
   using Bivouac.Abstractions;
   using Microsoft.AspNetCore.Http;
   using Microsoft.Extensions.Primitives;

   public class RequestIdGetter : IGetRequestId
   {
      private const string RequestIdKey = "request-id";

      private readonly IHttpContextAccessor _httpContextAccessor;
      private readonly IGenerateGuids _guidGenerator;

      public RequestIdGetter(
         IHttpContextAccessor httpContextAccessor,
         IGenerateGuids guidGenerator)
      {
         if (httpContextAccessor == null) throw new ArgumentNullException(nameof(httpContextAccessor));
         if (guidGenerator == null) throw new ArgumentNullException(nameof(guidGenerator));

         _httpContextAccessor = httpContextAccessor;
         _guidGenerator = guidGenerator;
      }

      public Guid Get()
      {
         var context = _httpContextAccessor.HttpContext;

         // Inspect HttpContext first...
         object idFromContext;
         if (context.Items.TryGetValue(RequestIdKey, out idFromContext))
         {
            return (Guid)idFromContext;
         }

         Guid id;

         // Then headers...
         StringValues idFromHeaders;
         if (context.Request.Headers.TryGetValue(RequestIdKey, out idFromHeaders))
         {
            id = Guid.Parse(idFromHeaders[0]);
         }
         else
         {
            // Create one if not in HttpContext or Headers
            id = _guidGenerator.Generate();
         }

         // Finally add to HttpContext
         context.Items.Add(RequestIdKey, id);
         return id;
      }
   }
}