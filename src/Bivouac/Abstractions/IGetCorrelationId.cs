namespace Bivouac.Abstractions
{
   using System;

   public interface IGetCorrelationId
   {
      Guid Get();
   }
}