using Bivouac.Abstractions;

namespace Bivouac.EventCallbacks
{
   public class NoOpHttpServerEventCallback : IHttpServerEventCallback
   {
      public void Invoke(IHttpServerEvent @event)
      {
      }
   }
}
