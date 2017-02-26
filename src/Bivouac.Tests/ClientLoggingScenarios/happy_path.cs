namespace Bivouac.Tests.ClientLoggingScenarios
{
   using System;
   using System.Linq;
   using System.Net.Http;
   using System.Runtime.InteropServices;
   using Xunit;
   using Shouldly;
   using Bivouac.Events;
   using Burble.Abstractions;
   using Newtonsoft.Json.Linq;

   public class happy_path
   {
      private readonly StubHttpClientEventCallback _callback;
      private readonly string _currentRequestId;
      private readonly string _newRequestId;
      private readonly string _correlationId;
      private readonly string _version;
      private readonly JObject _idsFromHeaders;

      public happy_path()
      {
         _currentRequestId = Guid.NewGuid().ToString();
         _newRequestId = Guid.NewGuid().ToString();
         _correlationId = Guid.NewGuid().ToString();
         _version = "1.0.1-client";

         var requestIdGetter = new StubRequestIdGetter { RequestId = _currentRequestId };
         var correlationIdGetter = new StubCorrelationIdGetter { CorrelationId = _correlationId };
         var assemblyVersionGetter = new StubAssemblyVersionGetter { Version = _version };
         var guidGenerator = new StubGuidGenerator(Guid.Parse(_newRequestId));

         _callback = new StubHttpClientEventCallback();
         var identifyingCallback = new IdentifyingHttpClientEventCallback(requestIdGetter, correlationIdGetter, assemblyVersionGetter, _callback);

         using (var webApi = new GetIdsFromHeadersApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new TestHttpClient(baseHttpClient, identifyingCallback)
               .AddIdentifyingHeaders(correlationIdGetter, guidGenerator, assemblyVersionGetter, "my-service");

            var response = httpClient.GetAsync("/get-ids-from-headers").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            _idsFromHeaders = JObject.Parse(content);
         }
      }
      
      [Fact]
      public void should_log_event_with_tags()
      {
         var lastRequest = _callback.Events.Single();
         lastRequest.Tags.ShouldNotBeNull();
         lastRequest.Tags.ShouldContainKeyAndValue("origin-request-id", _currentRequestId);
         lastRequest.Tags.ShouldContainKeyAndValue("correlation-id", _correlationId);
         lastRequest.Tags.ShouldContainKeyAndValue("request-id", _newRequestId);
         lastRequest.Tags.ShouldContainKeyAndValue("version", _version);
      }

      [Fact]
      public void new_request_id_is_added_to_the_headers()
      {
         _idsFromHeaders["requestId"].Value<string>().ShouldBe(_newRequestId);
      }

      // origin-request-id not needed in headers, only the events

      [Fact]
      public void correlation_id_is_added_to_the_headers()
      {
         _idsFromHeaders["correlationId"].Value<string>().ShouldBe(_correlationId);
      }

      [Fact]
      public void user_agent_is_added_to_the_headers()
      {
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe($"my-service/1.0.1-client ({RuntimeInformation.OSDescription.Trim()})");
      }
   }
}
