using Grouchy.HttpApi.Server.Model;

namespace Grouchy.HttpApi.Server.Abstractions
{
    public interface IStatusAvailabilityService
    {
        Availability GetAvailability(params Dependency[] dependencies);
    }
}