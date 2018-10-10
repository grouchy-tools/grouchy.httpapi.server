using Bivouac.Abstractions;
using Bivouac.Events;
using Bivouac.HttpClients;
using Burble.Abstractions;

namespace Bivouac.Extensions
{
   public static class HttpClientExtensions
   {
      public static IHttpClient AddIdentifyingHeaders(
         this IHttpClient httpClient,
         IGetCorrelationId correlationIdGetter,
         IGenerateGuids guidGenerator,
         IGetServiceName serviceNameGetter,
         IGetServiceVersion serviceVersionGetter,
         string environment = null)
      {
         return new IdentifyingHttpClient(
            httpClient,
            correlationIdGetter,
            guidGenerator,
            serviceNameGetter,
            serviceVersionGetter,
            environment);
      }
   }
}
