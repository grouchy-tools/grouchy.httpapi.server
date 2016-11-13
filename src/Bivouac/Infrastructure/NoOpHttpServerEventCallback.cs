namespace Bivouac.Infrastructure
{
   using Bivouac.Abstractions;

   public class NoOpHttpServerEventCallback : IHttpServerEventCallback
   {
      public void Invoke(IHttpServerEvent @event)
      {
      }
   }
}
