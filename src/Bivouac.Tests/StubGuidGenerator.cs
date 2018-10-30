using System;
using System.Collections.Generic;
using Burble.Abstractions;

namespace Bivouac.Tests
{
   public class StubGuidGenerator : IGenerateGuids
   {
      private readonly Queue<Guid> _guids;

      public StubGuidGenerator(params Guid[] guids)
      {
         _guids = new Queue<Guid>(guids);
      }

      public Guid Generate()
      {
         return _guids.Dequeue();
      }

      public void Add(params Guid[] guids)
      {
         foreach (var guid in guids)
         {
            _guids.Enqueue(guid);
         }
      }
   }
}