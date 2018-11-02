using System;
using Grouchy.Abstractions;

namespace Grouchy.HttpApi.Server.Services
{
   public class GuidGenerator : IGenerateGuids
   {
      public Guid Generate()
      {
         return Guid.NewGuid();
      }
   }
}