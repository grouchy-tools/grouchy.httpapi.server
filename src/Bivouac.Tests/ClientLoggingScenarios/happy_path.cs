namespace Bivouac.Tests.ClientLoggingScenarios
{
   using System;
   using System.Linq;
   using System.Net;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Microsoft.AspNetCore.Http;
   using Xunit;
   using Shouldly;
   using Banshee;
   using Bivouac.Events;
   using Burble;
   using Newtonsoft.Json;
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
         var correlatingCallback = new CorrelatingHttpClientEventCallback(requestIdGetter, correlationIdGetter, _callback);

         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new SimpleHttpClient(baseHttpClient)
               .AddLogging(correlatingCallback)
               .AddCorrelatingHeaders(correlationIdGetter, guidGenerator);

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

      private class PingWebApi : StubWebApiHost
      {
         protected override async Task Handler(HttpContext context)
         {
            if (context.Request.Method == "GET" && context.Request.Path.ToString() == "/get-ids-from-headers")
            {
               var response = new
               {
                  requestId = context.Request.Headers["request-id"].ToString(),
                  correlationId = context.Request.Headers["correlation-id"].ToString()
               };
               var json = JsonConvert.SerializeObject(response);

               context.Response.StatusCode = (int)HttpStatusCode.OK;
               await context.Response.WriteAsync(json);
            }
            else
            {
               await base.Handler(context);
            }
         }
      }
   }
}
