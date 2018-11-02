using Grouchy.HttpApi.Server.Abstractions;

namespace Grouchy.HttpApi.Server.EventCallbacks
{
   public class NoOpHttpServerEventCallback : IHttpServerEventCallback
   {
      public void Invoke(IHttpServerEvent @event)
      {
      }
   }
}
