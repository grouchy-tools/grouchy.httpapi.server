using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.Extensions;
using Grouchy.HttpApi.Client.Extensions;
using NUnit.Framework;
using Shouldly;
using Newtonsoft.Json.Linq;

namespace Grouchy.HttpApi.Server.Tests.ClientLoggingScenarios
{
   // ReSharper disable once InconsistentNaming
   public class service_environment_is_specified
   {
      private JObject _idsFromHeaders;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var correlationIdGetter = new StubCorrelationIdGetter();
         var guidGenerator = new StubGuidGenerator(Guid.NewGuid());
         var applicationInfo = new StubApplicationInfo { Name = "my-service", Environment = "Production", OperatingSystem = "my-os"};

         using (var webApi = new GetIdsFromHeadersApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new TestHttpClient(baseHttpClient)
               .AddIdentifyingHeaders(correlationIdGetter, guidGenerator, applicationInfo);

            var response = await httpClient.GetAsync("/get-ids-from-headers");
            var content = await response.Content.ReadAsStringAsync();
            _idsFromHeaders = JObject.Parse(content);
         }
      }
      
      [Test]
      public void user_agent_is_added_to_the_headers()
      {
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe("my-service Production (my-os)");
      }
   }
}
