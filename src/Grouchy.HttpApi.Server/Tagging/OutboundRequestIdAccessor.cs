using System.Threading;
using Grouchy.HttpApi.Client.Abstractions.Tagging;

namespace Grouchy.HttpApi.Server.Tagging
{
   public class OutboundRequestIdAccessor : IOutboundRequestIdAccessor
   {
      private static readonly AsyncLocal<string> OutboundRequestIdCurrent = new AsyncLocal<string>();

      public string OutboundRequestId
      {
         get => OutboundRequestIdCurrent.Value;
         set => OutboundRequestIdCurrent.Value = value;
      }
   }
}