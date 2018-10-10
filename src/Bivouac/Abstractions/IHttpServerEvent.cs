using Burble.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Bivouac.Abstractions
{
   public interface IHttpServerEvent : IEvent
   {
      string Uri { get; }

      string Method { get; }

      HttpRequest Request { get; }
   }
}