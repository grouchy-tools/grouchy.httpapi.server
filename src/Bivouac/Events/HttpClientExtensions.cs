namespace Bivouac.Events
{
   using Bivouac.Abstractions;
   using Burble.Abstractions;

   public static class HttpClientExtensions
   {
      public static IHttpClient AddCorrelatingHeaders(this IHttpClient httpClient, IGetCorrelationId correlationIdGetter, IGenerateGuids guidGenerator, string service, string version = null, string environment = null)
      {
         return new CorrelatingHttpClient(httpClient, correlationIdGetter, guidGenerator, service, version, environment);
      }
   }
}
