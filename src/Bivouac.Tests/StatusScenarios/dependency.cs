namespace Bivouac.Tests.StatusScenarios
{
   using System;
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;
   using Microsoft.Extensions.DependencyInjection;
   using Bivouac.Abstractions;
   using Bivouac.Model;

   public class dependency : IClassFixture<dependency.fixture>
   {
      public class fixture : StatusFixture
      {
         public readonly HttpResponseMessage Response;
         public readonly string Content;

         public fixture()
         {
            Response = TestHost.Get("/status");
            Content = Response.Content.ReadAsStringAsync().Result;
         }

         protected override void ConfigureServicesBuilder(IServiceCollection services)
         {
            base.ConfigureServicesBuilder(services);

            var status = new Status
            {
               Name = "level1",
               Availability = Availability.Available,
               Version = "version1",
               Dependencies = new[]
               {
                  new Status
                  {
                     Name="level2",
                     Version= "version2",
                     Availability = Availability.Available
                  }
               }
            };

            services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status });
         }
      }

      private readonly fixture _fixture;

      public dependency(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code_okay()
      {
         Assert.Equal(200, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_json_content_with_dependency()
      {
         var status = JsonConvert.DeserializeObject<Status>(_fixture.Content);

         Assert.Equal(Availability.Available, status.Availability);
         Assert.Equal(1, status.Dependencies.Length);
      }

      [Fact]
      public void should_return_level_1_dependency()
      {
         var status = JsonConvert.DeserializeObject<Status>(_fixture.Content);
         var level1 = status.Dependencies[0];

         Assert.Equal("level1", level1.Name);
         Assert.Equal(Availability.Available, level1.Availability);
         Assert.Equal("version1", level1.Version);
         Assert.Equal(1, level1.Dependencies.Length);
      }

      [Fact]
      public void should_return_level_2_dependency()
      {
         var status = JsonConvert.DeserializeObject<Status>(_fixture.Content);
         var level2 = status.Dependencies[0].Dependencies[0];

         Assert.Equal("level2", level2.Name);
         Assert.Equal(Availability.Available, level2.Availability);
         Assert.Equal("version2", level2.Version);
         Assert.Null(level2.Dependencies);
      }
   }
}
