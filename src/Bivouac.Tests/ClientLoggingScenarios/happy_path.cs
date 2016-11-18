namespace Bivouac.Tests.ClientLoggingScenarios
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

   public class happy_path
   {
      private readonly StubHttpClientEventCallback _callback;
      private readonly string _currentRequestId;
      private readonly string _newRequestId;
      private readonly string _correlationId;
      private readonly JObject _idsFromHeaders;

      public happy_path()
      {
         _currentRequestId = Guid.NewGuid().ToString();
         _newRequestId = Guid.NewGuid().ToString();
         _correlationId = Guid.NewGuid().ToString();

         var requestIdGetter = new StubRequestIdGetter { RequestId = _currentRequestId };
         var correlationIdGetter = new StubCorrelationIdGetter { CorrelationId = _correlationId };
         var guidGenerator = new StubGuidGenerator(Guid.Parse(_newRequestId));

         _callback = new StubHttpClientEventCallback();
         var identifyingCallback = new IdentifyingHttpClientEventCallback(requestIdGetter, correlationIdGetter, _callback);

         using (var webApi = new GetIdsFromHeadersApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new SimpleHttpClient(baseHttpClient)
               .AddLogging(identifyingCallback)
               .AddIdentifyingHeaders(correlationIdGetter, guidGenerator, "my-service");

            var response = httpClient.GetAsync("/get-ids-from-headers").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            _idsFromHeaders = JObject.Parse(content);
         }
      }
      
      [Fact]
      public void should_log_request_initiated()
      {
         var lastRequest = _callback.RequestsInitiated.Last();
         lastRequest.Tags.ShouldNotBeNull();
         lastRequest.Tags.ShouldContainKeyAndValue("origin-request-id", _currentRequestId);
         lastRequest.Tags.ShouldContainKeyAndValue("correlation-id", _correlationId);
         lastRequest.Tags.ShouldContainKeyAndValue("request-id", _newRequestId);
      }

      [Fact]
      public void should_log_response_received()
      {
         var lastResponse = _callback.ResponsesReceived.Last();
         lastResponse.Tags.ShouldNotBeNull();
         lastResponse.Tags.ShouldContainKeyAndValue("origin-request-id", _currentRequestId);
         lastResponse.Tags.ShouldContainKeyAndValue("correlation-id", _correlationId);
         lastResponse.Tags.ShouldContainKeyAndValue("request-id", _newRequestId);
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
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe($"my-service ({RuntimeInformation.OSDescription.Trim()})");
      }
   }
}
