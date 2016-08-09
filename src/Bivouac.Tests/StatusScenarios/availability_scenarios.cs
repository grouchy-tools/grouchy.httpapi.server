namespace Bivouac.Tests.StatusScenarios
{
   using Bivouac.Services;
   using Bivouac.Model;
   using Xunit;

   public class availability_scenarios
   {
      [Fact]
      public void no_dependencies()
      {
         var testSubject = new StatusAvailabilityService();

         var availability = testSubject.GetAvailability();

         Assert.Equal(Availability.Available, availability);
      }

      [Theory]
      [InlineData(Availability.Available, Availability.Available)]
      [InlineData(Availability.Unavailable, Availability.Limited)]
      [InlineData(Availability.Limited, Availability.Limited)]
      [InlineData(Availability.Unknown, Availability.Unknown)]
      public void single_dependency(Availability dependencyAvailability, Availability expectedAvailability)
      {
         var testSubject = new StatusAvailabilityService();

         var availability = testSubject.GetAvailability(new Status { Availability = dependencyAvailability });

         Assert.Equal(expectedAvailability, availability);
      }

      [Theory]
      [InlineData(Availability.Available, Availability.Available, Availability.Available)]
      [InlineData(Availability.Available, Availability.Unavailable, Availability.Limited)]
      [InlineData(Availability.Available, Availability.Limited, Availability.Limited)]
      [InlineData(Availability.Available, Availability.Unknown, Availability.Unknown)]
      [InlineData(Availability.Unavailable, Availability.Available, Availability.Limited)]
      [InlineData(Availability.Unavailable, Availability.Unavailable, Availability.Limited)]
      [InlineData(Availability.Unavailable, Availability.Limited, Availability.Limited)]
      [InlineData(Availability.Unavailable, Availability.Unknown, Availability.Limited)]
      [InlineData(Availability.Limited, Availability.Available, Availability.Limited)]
      [InlineData(Availability.Limited, Availability.Unavailable, Availability.Limited)]
      [InlineData(Availability.Limited, Availability.Limited, Availability.Limited)]
      [InlineData(Availability.Limited, Availability.Unknown, Availability.Limited)]
      [InlineData(Availability.Unknown, Availability.Available, Availability.Unknown)]
      [InlineData(Availability.Unknown, Availability.Unavailable, Availability.Limited)]
      [InlineData(Availability.Unknown, Availability.Limited, Availability.Limited)]
      [InlineData(Availability.Unknown, Availability.Unknown, Availability.Unknown)]
      public void single_dependency(Availability dependencyAvailability1, Availability dependencyAvailability2, Availability expectedAvailability)
      {
         var testSubject = new StatusAvailabilityService();

         var availability = testSubject.GetAvailability(new Status { Availability = dependencyAvailability1 }, new Status { Availability = dependencyAvailability2 });

         Assert.Equal(expectedAvailability, availability);
      }
   }
}
