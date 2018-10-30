using System;
using Burble.Abstractions;

namespace Bivouac.Services
{
   public class GuidGenerator : IGenerateGuids
   {
      public Guid Generate()
      {
         return Guid.NewGuid();
      }
   }
}