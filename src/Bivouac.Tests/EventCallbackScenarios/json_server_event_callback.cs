using System;
using System.Collections.Generic;
using Bivouac.Abstractions;
using Bivouac.EventCallbacks;
using Bivouac.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Shouldly;
using NUnit.Framework;

namespace Bivouac.Tests.EventCallbackScenarios
{
   // ReSharper disable once InconsistentNaming
   public class json_server_event_callback
   {
      private StubHttpContext _httpContext;
      private StubLogger<JsonLoggingHttpServerEventCallback> _logger;
      private IHttpServerEventCallback _testSubject;

      [SetUp]
      public void setup_before_each_test()
      {
         IHeaderDictionary headers = new HeaderDictionary(new Dictionary<string, StringValues> { { "User-Agent", new StringValues("my-user-agent") } });
         var httpRequest = new StubHttpRequest(headers) { Method = "GET", Path = "/ping", QueryString = new QueryString("?v=1") };
         var httpResponse = new StubHttpResponse { StatusCode = 23 };
         _httpContext = new StubHttpContext(httpRequest, httpResponse);
         _logger = new StubLogger<JsonLoggingHttpServerEventCallback>();

         _testSubject = new JsonLoggingHttpServerEventCallback(_logger);
      }

      [Test]
      public void logs_one_item()
      {
         var httpServerRequest = HttpServerRequest.Create(_httpContext);

         _testSubject.Invoke(httpServerRequest);

         _logger.Logs.Count.ShouldBe(1);
      }

      [Test]
      public void serialise_server_request()
      {
         var httpServerRequest = HttpServerRequest.Create(_httpContext);
         httpServerRequest.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);

         _testSubject.Invoke(httpServerRequest);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"HttpServerRequest\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"/ping?v=1\",\"method\":\"GET\",\"userAgent\":\"my-user-agent\"}");
      }

      [Test]
      public void serialise_server_request_with_tag()
      {
         var httpServerRequest = HttpServerRequest.Create(_httpContext);
         httpServerRequest.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);
         httpServerRequest.Tags.Add("key", "value");

         _testSubject.Invoke(httpServerRequest);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"HttpServerRequest\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"/ping?v=1\",\"method\":\"GET\",\"tags\":{\"key\":\"value\"},\"userAgent\":\"my-user-agent\"}");
      }

      [Test]
      public void serialise_server_response()
      {
         var httpServerResponse = HttpServerResponse.Create(_httpContext, 1358);
         httpServerResponse.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);

         _testSubject.Invoke(httpServerResponse);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"HttpServerResponse\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"/ping?v=1\",\"method\":\"GET\",\"statusCode\":23,\"durationMs\":1358}");
      }

      [Test]
      public void serialise_server_response_with_tag()
      {
         var httpServerResponse = HttpServerResponse.Create(_httpContext, 1358);
         httpServerResponse.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);
         httpServerResponse.Tags.Add("key", "value");

         _testSubject.Invoke(httpServerResponse);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"HttpServerResponse\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"/ping?v=1\",\"method\":\"GET\",\"tags\":{\"key\":\"value\"},\"statusCode\":23,\"durationMs\":1358}");
      }

      [Test]
      public void serialise_server_exception()
      {
         var httpServerResponse = HttpServerException.Create(_httpContext, new ExceptionWithStackTrace("my-exception") { });
         httpServerResponse.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);

         _testSubject.Invoke(httpServerResponse);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"HttpServerException\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"/ping?v=1\",\"method\":\"GET\",\"exception\":{\"type\":\"ExceptionWithStackTrace\",\"message\":\"my-exception\",\"stackTrace\":\"the-stack-trace\"}}");
      }

      [Test]
      public void serialise_server_exception_with_inner_exception()
      {
         var httpServerResponse = HttpServerException.Create(_httpContext, new Exception("my-exception", new ApplicationException("inner")));
         httpServerResponse.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);

         _testSubject.Invoke(httpServerResponse);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"HttpServerException\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"/ping?v=1\",\"method\":\"GET\",\"exception\":{\"type\":\"Exception\",\"message\":\"my-exception\",\"innerException\":{\"type\":\"ApplicationException\",\"message\":\"inner\"}}}");
      }

      [Test]
      public void serialise_server_exception_with_tag()
      {
         var httpServerResponse = HttpServerException.Create(_httpContext, new Exception("my-exception"));
         httpServerResponse.Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454);
         httpServerResponse.Tags.Add("key1", "value1");
         httpServerResponse.Tags.Add("key2", "value2");

         _testSubject.Invoke(httpServerResponse);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"HttpServerException\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"uri\":\"/ping?v=1\",\"method\":\"GET\",\"tags\":{\"key1\":\"value1\",\"key2\":\"value2\"},\"exception\":{\"type\":\"Exception\",\"message\":\"my-exception\"}}");
      }

      private class ExceptionWithStackTrace : Exception
      {
         public ExceptionWithStackTrace(string message) : base(message)
         {
         }
         
         public override string StackTrace { get; } = "the-stack-trace";
      }
   }
}
