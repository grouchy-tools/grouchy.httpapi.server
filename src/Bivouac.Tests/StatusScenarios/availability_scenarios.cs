using Bivouac.Services;
using Bivouac.Model;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios
{
   // ReSharper disable once InconsistentNaming
   public class availability_scenarios
   {
      [Test]
      public void no_dependencies()
      {
         var testSubject = new StatusAvailabilityService();

         var availability = testSubject.GetAvailability();

         Assert.AreEqual(Availability.Available, availability);
      }

      [TestCase(Availability.Available, Availability.Available)]
      [TestCase(Availability.Unavailable, Availability.Limited)]
      [TestCase(Availability.Limited, Availability.Limited)]
      [TestCase(Availability.Unknown, Availability.Unknown)]
      public void single_dependency(Availability dependencyAvailability, Availability expectedAvailability)
      {
         var testSubject = new StatusAvailabilityService();

         var availability = testSubject.GetAvailability(new Dependency { Availability = dependencyAvailability });

         Assert.AreEqual(expectedAvailability, availability);
      }

      [TestCase(Availability.Available, Availability.Available, Availability.Available)]
      [TestCase(Availability.Available, Availability.Unavailable, Availability.Limited)]
      [TestCase(Availability.Available, Availability.Limited, Availability.Limited)]
      [TestCase(Availability.Available, Availability.Unknown, Availability.Unknown)]
      [TestCase(Availability.Unavailable, Availability.Available, Availability.Limited)]
      [TestCase(Availability.Unavailable, Availability.Unavailable, Availability.Limited)]
      [TestCase(Availability.Unavailable, Availability.Limited, Availability.Limited)]
      [TestCase(Availability.Unavailable, Availability.Unknown, Availability.Limited)]
      [TestCase(Availability.Limited, Availability.Available, Availability.Limited)]
      [TestCase(Availability.Limited, Availability.Unavailable, Availability.Limited)]
      [TestCase(Availability.Limited, Availability.Limited, Availability.Limited)]
      [TestCase(Availability.Limited, Availability.Unknown, Availability.Limited)]
      [TestCase(Availability.Unknown, Availability.Available, Availability.Unknown)]
      [TestCase(Availability.Unknown, Availability.Unavailable, Availability.Limited)]
      [TestCase(Availability.Unknown, Availability.Limited, Availability.Limited)]
      [TestCase(Availability.Unknown, Availability.Unknown, Availability.Unknown)]
      public void multi_dependency(Availability dependencyAvailability1, Availability dependencyAvailability2, Availability expectedAvailability)
      {
         var testSubject = new StatusAvailabilityService();

         var availability = testSubject.GetAvailability(new Dependency { Availability = dependencyAvailability1 }, new Dependency { Availability = dependencyAvailability2 });

         Assert.AreEqual(expectedAvailability, availability);
      }
   }
}
