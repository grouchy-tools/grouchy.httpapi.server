namespace Bivouac
{
   using Bivouac.Abstractions;
   using Bivouac.Events;
   using Burble.Abstractions;

   public static class HttpClientExtensions
   {
      public static IHttpClient AddIdentifyingHeaders(this IHttpClient httpClient, IGetCorrelationId correlationIdGetter, IGenerateGuids guidGenerator, IGetAssemblyVersion assemblyVersionGetter, string service, string environment = null)
      {
         return new IdentifyingHttpClient(httpClient, correlationIdGetter, guidGenerator, assemblyVersionGetter, service, environment);
      }
   }
}
