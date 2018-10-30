using Bivouac.Model;

namespace Bivouac.Abstractions
{
    public interface IStatusAvailabilityService
    {
        Availability GetAvailability(params Dependency[] dependencies);
    }
}