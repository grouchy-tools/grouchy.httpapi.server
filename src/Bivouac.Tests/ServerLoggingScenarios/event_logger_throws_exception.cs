namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System;
   using System.Net.Http;
   using Xunit;

   public class event_logger_throws_exception : IClassFixture<event_logger_throws_exception.fixture>
   {
      public class fixture : ServerLoggingFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            StubEventLogger.Exception = new Exception();

            Response = TestHost.Get("/exception");
         }
      }

      private readonly fixture _fixture;

      public event_logger_throws_exception(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code_500()
      {
         Assert.Equal(500, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_content()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         Assert.Equal("FAIL!", content);
      }

      [Fact]
      public void should_return_content_type_text_plain()
      {
         var contentType = _fixture.Response.Content.Headers.ContentType.MediaType;

         Assert.Equal("text/plain", contentType);
      }
   }
}
