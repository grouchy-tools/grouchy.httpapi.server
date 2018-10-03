﻿using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using Shouldly;
using Bivouac.Events;
using Burble.Abstractions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Bivouac.Tests.ClientLoggingScenarios
{
   public class happy_path
   {
      private StubHttpClientEventCallback _callback;
      private string _currentRequestId;
      private string _newRequestId;
      private string _correlationId;
      private string _version;
      private JObject _idsFromHeaders;

      [OneTimeSetUp]
      public void setup_scenario()
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
      
      [Test]
      public void should_log_event_with_tags()
      {
         var lastRequest = _callback.Events.Single();
         lastRequest.Tags.ShouldNotBeNull();
         lastRequest.Tags.ShouldContainKeyAndValue("origin-request-id", _currentRequestId);
         lastRequest.Tags.ShouldContainKeyAndValue("correlation-id", _correlationId);
         lastRequest.Tags.ShouldContainKeyAndValue("request-id", _newRequestId);
         lastRequest.Tags.ShouldContainKeyAndValue("version", _version);
      }

      [Test]
      public void new_request_id_is_added_to_the_headers()
      {
         _idsFromHeaders["requestId"].Value<string>().ShouldBe(_newRequestId);
      }

      // origin-request-id not needed in headers, only the events

      [Test]
      public void correlation_id_is_added_to_the_headers()
      {
         _idsFromHeaders["correlationId"].Value<string>().ShouldBe(_correlationId);
      }

      [Test]
      public void user_agent_is_added_to_the_headers()
      {
         _idsFromHeaders["userAgent"].Value<string>().ShouldBe($"my-service/1.0.1-client ({RuntimeInformation.OSDescription.Trim()})");
      }
   }
}
