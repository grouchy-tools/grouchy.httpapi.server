namespace Bivouac.Infrastructure
{
   using Burble.Abstractions;

   public class NoOpHttpClientEventCallback : IHttpClientEventCallback
   {
      public void Invoke(IHttpClientEvent @event)
      {
      }
   }
}
