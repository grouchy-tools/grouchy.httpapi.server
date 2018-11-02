namespace Grouchy.HttpApi.Server.Abstractions
{
   public interface IHttpServerEventCallback
   {
      void Invoke(IHttpServerEvent @event);
   }
}