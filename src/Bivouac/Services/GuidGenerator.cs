namespace Bivouac.Services
{
   using System;
   using Bivouac.Abstractions;

   public class GuidGenerator : IGenerateGuids
   {
      public Guid Generate()
      {
         return Guid.NewGuid();
      }
   }
}