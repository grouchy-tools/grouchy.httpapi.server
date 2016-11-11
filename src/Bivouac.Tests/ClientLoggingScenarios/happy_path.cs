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
      private readonly string _correlationId;
      private readonly JObject _idsFromHeaders;

      public happy_path()
      {
         _currentRequestId = Guid.NewGuid().ToString();
         _correlationId = Guid.NewGuid().ToString();

         var requestIdGetter = new StubRequestIdGetter { RequestId = _currentRequestId };
         var correlationIdGetter = new StubCorrelationIdGetter { CorrelationId = _correlationId };

         _callback = new StubHttpClientEventCallback(new CorrelatingHttpClientEventCallback(requestIdGetter, correlationIdGetter));

         using (var webApi = new PingWebApi())
         using (var baseHttpClient = new HttpClient { BaseAddress = webApi.BaseUri })
         {
            var httpClient = new SimpleHttpClient(baseHttpClient)
               .AddCorrelatingHeaders(correlationIdGetter)
               .AddLogging(_callback);

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
         lastRequest.Tags.ShouldContainKeyAndValue("originRequestId", _currentRequestId);
         lastRequest.Tags.ShouldContainKeyAndValue("correlationId", _correlationId);
      }

      [Fact]
      public void should_log_response_received()
      {
         var lastResponse = _callback.ResponsesReceived.Last();
         lastResponse.Tags.ShouldNotBeNull();
         lastResponse.Tags.ShouldContainKeyAndValue("originRequestId", _currentRequestId);
         lastResponse.Tags.ShouldContainKeyAndValue("correlationId", _correlationId);
      }

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
               var response = new { requestId = context.Request.Headers["request-id"].ToString(), correlationId = context.Request.Headers["correlation-id"].ToString() };
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
