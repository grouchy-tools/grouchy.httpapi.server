namespace Bivouac.Abstractions
{
   using Bivouac.Model;

   public interface IStatusEndpointService
   {
      string GetName();

      string GetVersion();

      string GetBuild();
   }

   public interface IStatusAvailabilityService
   {
      Availability GetAvailability(params Status[] dependencies);
   }
}