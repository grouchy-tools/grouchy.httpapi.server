namespace Bivouac.Services
{
   using System.Linq;
   using Bivouac.Abstractions;
   using Bivouac.Model;

   public class StatusAvailabilityService : IStatusAvailabilityService
   {
      public Availability GetAvailability(params Status[] dependencies)
      {
         if (dependencies == null || dependencies.Length == 0)
         {
            return Availability.Available;
         }

         if (dependencies.Any(c => c.Availability == Availability.Unavailable || c.Availability == Availability.Limited))
         {
            return Availability.Limited;
         }

         if (dependencies.Any(c => c.Availability == Availability.Unknown))
         {
            return Availability.Unknown;
         }

         return Availability.Available;
      }
   }
}