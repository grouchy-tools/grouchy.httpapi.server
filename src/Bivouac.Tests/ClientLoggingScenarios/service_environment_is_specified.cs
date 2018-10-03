using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Shouldly;
using Burble.Abstractions;
using Newtonsoft.Json.Linq;

namespace Bivouac.Tests.ClientLoggingScenarios
{
   public class service_environment_is_specified
   {
      private JObject _idsFromHeaders;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         var correlationIdGetter = new StubCorrelationIdGetter();
         var guidGenerator = new StubGuidGenerator(Guid.NewGuid());
         var assemblyVersionGetter = new StubAssemblyVersionGetter();

         using (var webApi = new GetIdsFromHeadersApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new TestHttpClient(baseHttpClient, null)
               .AddIdentifyingHeaders(correlationIdGetter, guidGenerator, assemblyVersionGetter, "my-service", environment: "Production");

            var response = httpClient.GetAsync("/get-ids-from-headers").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            _idsFromHeaders = JObject.Parse(content);
         }
      }
      
      [Test]
      public void user_agent_is_added_to_the_headers()
      {
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe($"my-service Production ({RuntimeInformation.OSDescription.Trim()})");
      }
   }
}
