namespace Bivouac.Abstractions
{
   using Bivouac.Model;

   public interface IStatusEndpointService
   {
      string GetName();

      string GetVersion();
   }

   public interface IStatusAvailabilityService
   {
      Availability GetAvailability(params Status[] dependencies);
   }
}