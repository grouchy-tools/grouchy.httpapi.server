namespace Bivouac.Tests.ClientLoggingScenarios
{
   using System;
   using System.Net.Http;
   using System.Runtime.InteropServices;
   using Xunit;
   using Shouldly;
   using Burble.Abstractions;
   using Newtonsoft.Json.Linq;

   public class service_version_and_environment_are_specified
   {
      private readonly JObject _idsFromHeaders;

      public service_version_and_environment_are_specified()
      {
         var correlationIdGetter = new StubCorrelationIdGetter();
         var guidGenerator = new StubGuidGenerator(Guid.NewGuid());
         var assemblyVersionGetter = new StubAssemblyVersionGetter { Version = "1.1.2" };

         using (var webApi = new GetIdsFromHeadersApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new TestHttpClient(baseHttpClient, null)
               .AddIdentifyingHeaders(correlationIdGetter, guidGenerator, assemblyVersionGetter, "my-service", "Staging");

            var response = httpClient.GetAsync("/get-ids-from-headers").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            _idsFromHeaders = JObject.Parse(content);
         }
      }
      
      [Fact]
      public void user_agent_is_added_to_the_headers()
      {
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe($"my-service/1.1.2 Staging ({RuntimeInformation.OSDescription.Trim()})");
      }
   }
}
