using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using Bivouac.Extensions;
using Burble.Abstractions.Extensions;
using NUnit.Framework;
using Shouldly;
using Newtonsoft.Json.Linq;

namespace Bivouac.Tests.ClientLoggingScenarios
{
   public class service_version_and_environment_are_specified
   {
      private JObject _idsFromHeaders;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         var correlationIdGetter = new StubCorrelationIdGetter();
         var guidGenerator = new StubGuidGenerator(Guid.NewGuid());
         var serviceNameGetter = new StubServiceNameGetter { Name = "my-service" };
         var assemblyVersionGetter = new StubServiceVersionGetter { Version = "1.1.2" };

         using (var webApi = new GetIdsFromHeadersApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new TestHttpClient(baseHttpClient)
               .AddIdentifyingHeaders(correlationIdGetter, guidGenerator, serviceNameGetter, assemblyVersionGetter, environment: "Staging");

            var response = httpClient.GetAsync("/get-ids-from-headers").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            _idsFromHeaders = JObject.Parse(content);
         }
      }
      
      [Test]
      public void user_agent_is_added_to_the_headers()
      {
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe($"my-service/1.1.2 Staging ({RuntimeInformation.OSDescription.Trim()})");
      }
   }
}
