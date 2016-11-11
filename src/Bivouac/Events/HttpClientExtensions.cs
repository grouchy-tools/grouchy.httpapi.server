namespace Bivouac.Events
{
   using Bivouac.Abstractions;
   using Burble.Abstractions;

   public static class HttpClientExtensions
   {
      public static IHttpClient AddCorrelatingHeaders(this IHttpClient httpClient, IGetCorrelationId correlationIdGetter)
      {
         return new CorrelatingHttpClient(httpClient, correlationIdGetter);
      }
   }
}
