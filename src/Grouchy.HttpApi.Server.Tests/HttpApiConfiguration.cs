using System;
using Grouchy.HttpApi.Client.Abstractions.Configuration;

namespace Grouchy.HttpApi.Server.Tests
{
   public class HttpApiConfiguration : IHttpApiConfiguration
   {
      public string Name { get; set; }
      
      public Uri Uri { get; set; }

      public int? TimeoutMs { get; set; }
   }
}