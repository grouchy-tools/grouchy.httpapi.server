namespace Bivouac.Abstractions
{
   public interface IHttpServerEventCallback
   {
      void Invoke(IHttpServerEvent @event);
   }
}