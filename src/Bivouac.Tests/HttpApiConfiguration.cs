using System;
using Burble.Abstractions.Configuration;

namespace Bivouac.Tests
{
   public class HttpApiConfiguration : IHttpApiConfiguration
   {
      public string Name { get; set; }
      
      public Uri Uri { get; set; }

      public int? TimeoutMs { get; set; }
   }
}