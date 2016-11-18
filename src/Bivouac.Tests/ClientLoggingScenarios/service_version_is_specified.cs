﻿namespace Bivouac.Tests.ClientLoggingScenarios
{
   using System;
   using System.Linq;
   using System.Net.Http;
   using System.Runtime.InteropServices;
   using Xunit;
   using Shouldly;
   using Bivouac.Events;
   using Burble;
   using Newtonsoft.Json.Linq;

   public class service_version_is_specified
   {
      private readonly JObject _idsFromHeaders;

      public service_version_is_specified()
      {
         var correlationIdGetter = new StubCorrelationIdGetter();
         var guidGenerator = new StubGuidGenerator(Guid.NewGuid());

         using (var webApi = new GetIdsFromHeadersApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new SimpleHttpClient(baseHttpClient)
               .AddCorrelatingHeaders(correlationIdGetter, guidGenerator, "my-service", "1.0");

            var response = httpClient.GetAsync("/get-ids-from-headers").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            _idsFromHeaders = JObject.Parse(content);
         }
      }
      
      [Fact]
      public void user_agent_is_added_to_the_headers()
      {
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe($"my-service/1.0 ({RuntimeInformation.OSDescription.Trim()})");
      }
   }
}