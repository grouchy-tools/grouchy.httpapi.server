﻿namespace Bivouac.Tests.StatusScenarios
{
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;
   using Bivouac.Model;

   public class happy_path : IClassFixture<happy_path.fixture>
   {
      public class fixture : StatusFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            StubStatusEndpointService.Name = "myName";
            StubStatusEndpointService.Version = "myVersion";
            StubStatusEndpointService.Build = "myBuild";

            Response = TestHost.Get("/status");
         }
      }

      private readonly fixture _fixture;

      public happy_path(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code_okay()
      {
         Assert.Equal(200, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_json_content()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;
         var status = JsonConvert.DeserializeObject<Status>(content);

         Assert.Equal("myName", status.Name);
         Assert.Equal(Availability.Available, status.Availability);
         Assert.Equal("myVersion", status.Version);
         Assert.Equal("myBuild", status.Build);
      }

      [Fact]
      public void should_return_exact_json_content()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         Assert.Equal("{\"name\":\"myName\",\"availability\":\"Available\",\"version\":\"myVersion\",\"build\":\"myBuild\"}", content);
      }
   }
}
