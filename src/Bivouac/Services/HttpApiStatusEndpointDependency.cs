using System;
using System.Threading;
using System.Threading.Tasks;
using Bivouac.Abstractions;
using Bivouac.Model;
using Burble.Abstractions;
using Burble.Abstractions.Configuration;
using Burble.Abstractions.Exceptions;
using Burble.Abstractions.Extensions;
using Newtonsoft.Json;

namespace Bivouac.Services
{
   public class HttpApiStatusEndpointDependency : IStatusEndpointDependency
   {
      private readonly IHttpClient _httpClient;
      private readonly IHttpApiConfiguration _httpApiConfiguration;

      public HttpApiStatusEndpointDependency(
         IHttpClient httpClient,
         IHttpApiConfiguration httpApiConfiguration)
      {
         _httpClient = httpClient;
         _httpApiConfiguration = httpApiConfiguration;
      }

      public string Name => _httpApiConfiguration.Name;

      public async Task<Dependency> GetStatusAsync(CancellationToken cancellationToken)
      {
         try
         {
            var response = await _httpClient.GetAsync(".ping", cancellationToken);

            response.EnsureSuccessStatusCode();

            return new Dependency { Name = Name, Availability = Availability.Available };
         }
         catch (HttpClientTimeoutException)
         {
            return new Dependency { Name = Name, Availability = Availability.Unknown };
         }
         catch (TaskCanceledException)
         {
            return new Dependency { Name = Name, Availability = Availability.Unknown };
         }
         catch (Exception)
         {               
            return new Dependency { Name = Name, Availability = Availability.Unavailable };
         }
      }
   }
}
