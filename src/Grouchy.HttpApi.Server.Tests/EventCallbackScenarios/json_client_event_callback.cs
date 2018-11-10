using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Grouchy.HttpApi.Server.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Server.Tests.EventCallbackScenarios
{
   // ReSharper disable once InconsistentNaming
   public class json_client_event_callback
   {
      private Uri _baseAddress;
      private HttpRequestMessage _request;
      private HttpResponseMessage _response;
      private StubLogger<JsonLoggingHttpClientEventCallback> _logger;
      private IHttpClientEventCallback _testSubject;

      [SetUp]
      public void setup_before_each_test()
      {
         _baseAddress = new Uri("http://localhost:8080");
         _request = new HttpRequestMessage(HttpMethod.Get, "/ping");
         _response = new HttpResponseMessage(HttpStatusCode.Accepted) { RequestMessage = _request };
         _logger = new StubLogger<JsonLoggingHttpClientEventCallback>();

         _testSubject = new JsonLoggingHttpClientEventCallback(_logger);
      }

      [Test]
      public void logs_one_item()
      {
         var clientRequest = new StubHttpClientEvent();

         _testSubject.Invoke(clientRequest);

         _logger.Logs.Count.ShouldBe(1);
      }

      [Test]
      public void serialise_simple_client_event()
      {
         var clientRequest = new StubHttpClientEvent
            {
               EventType = "Simple",
               Request = _request,
               Method = "GET",
               Uri = $"{_baseAddress}ping",
               Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454)
            };

         _testSubject.Invoke(clientRequest);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"Simple\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"method\":\"GET\",\"uri\":\"http://localhost:8080/ping\"}");
      }

      [Test]
      public void serialise_simple_client_event_with_tag()
      {
         var clientRequest = new StubHttpClientEvent
         {
            EventType = "SimpleWithTag",
            Request = _request,
            Method = "GET",
            Uri = $"{_baseAddress}ping",
            Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454),
            Tags = new Dictionary<string, object>
            {
               {"key", "value"}
            }
         };

         _testSubject.Invoke(clientRequest);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"SimpleWithTag\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"method\":\"GET\",\"uri\":\"http://localhost:8080/ping\",\"tags\":{\"key\":\"value\"}}");
      }

      [Test]
      public void serialise_simple_client_event_with_additional_field()
      {
         var clientRequest = new StubHttpClientEvent
         {
            EventType = "SimpleWithField",
            Request = _request,
            Method = "POST",
            Uri = $"{_baseAddress}ping",
            Timestamp = new DateTimeOffset(2016, 11, 18, 19, 52, 6, TimeSpan.Zero).AddTicks(4425454),
            SomethingElse = "another"
         };

         _testSubject.Invoke(clientRequest);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"SimpleWithField\",\"timestamp\":\"2016-11-18T19:52:06.4425454+00:00\",\"method\":\"POST\",\"uri\":\"http://localhost:8080/ping\",\"somethingElse\":\"another\"}");
      }

      [Test]
      public void serialise_client_event_with_exception()
      {
         var clientRequest = new StubHttpClientEvent
         {
            EventType = "SimpleWithException",
            Exception = new ExceptionWithStackTrace("the-message")
         };

         _testSubject.Invoke(clientRequest);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"SimpleWithException\",\"timestamp\":\"0001-01-01T00:00:00+00:00\",\"exception\":{\"type\":\"ExceptionWithStackTrace\",\"message\":\"the-message\",\"stackTrace\":\"the-stack-trace\"}}");
      }

      [Test]
      public void serialise_client_event_with_inner_exception()
      {
         var clientRequest = new StubHttpClientEvent
         {
            EventType = "SimpleWithNestedException",
            Exception = new Exception("the-message", new ApplicationException("inner"))
         };

         _testSubject.Invoke(clientRequest);

         _logger.Logs[0].ShouldBe("{\"eventType\":\"SimpleWithNestedException\",\"timestamp\":\"0001-01-01T00:00:00+00:00\",\"exception\":{\"type\":\"Exception\",\"message\":\"the-message\",\"innerException\":{\"type\":\"ApplicationException\",\"message\":\"inner\"}}}");
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
