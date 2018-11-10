using System;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Server.EventCallbacks;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Server.Tests.EventCallbackScenarios
{
   // ReSharper disable once InconsistentNaming
   public class identifying_client_event_callback
   {
      private StubSessionIdAccessor _sessionIdAccessor;
      private StubCorrelationIdAccessor _correlationIdAccessor;
      private StubInboundRequestIdAccessor _inboundRequestIdAccessor;
      private StubOutboundRequestIdAccessor _outboundRequestIdAccessor;
      private StubApplicationInfo _applicationInfo;
      private IHttpClientEventCallback _testSubject;
      private string _outboundRequestId;
      private string _inboundRequestId;
      private string _correlationId;
      private string _sessionId;

      [SetUp]
      public void setup_before_each_test()
      {
         _outboundRequestId = Guid.NewGuid().ToString();
         _inboundRequestId = Guid.NewGuid().ToString();
         _correlationId = Guid.NewGuid().ToString();
         _sessionId = Guid.NewGuid().ToString();
         
         _sessionIdAccessor = new StubSessionIdAccessor {Response = _sessionId};
         _correlationIdAccessor = new StubCorrelationIdAccessor {Response = _correlationId};
         _inboundRequestIdAccessor = new StubInboundRequestIdAccessor {Response = _inboundRequestId};
         _outboundRequestIdAccessor = new StubOutboundRequestIdAccessor {Response = _outboundRequestId};
         _applicationInfo = new StubApplicationInfo {Name = "appName", Version = "appVersion"};
         
         _testSubject = new IdentifyingHttpClientEventCallback(_sessionIdAccessor, _correlationIdAccessor, _inboundRequestIdAccessor, _outboundRequestIdAccessor, _applicationInfo);
      }

      [Test]
      public void should_log_event_with_tags()
      {
         var lastRequest = new StubHttpClientEvent();
         
         _testSubject.Invoke(lastRequest);

         lastRequest.Tags.ShouldNotBeNull();
         lastRequest.Tags.ShouldContainKeyAndValue("outboundRequestId", _outboundRequestId);
         lastRequest.Tags.ShouldContainKeyAndValue("inboundRequestId", _inboundRequestId);
         lastRequest.Tags.ShouldContainKeyAndValue("correlationId", _correlationId);
         lastRequest.Tags.ShouldContainKeyAndValue("sessionId", _sessionId);
         lastRequest.Tags.ShouldContainKeyAndValue("service", "appName");
         lastRequest.Tags.ShouldContainKeyAndValue("version", "appVersion");
      }
   }
}
