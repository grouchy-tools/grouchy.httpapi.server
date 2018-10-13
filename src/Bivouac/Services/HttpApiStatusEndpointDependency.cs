using System;
using System.Threading;
using System.Threading.Tasks;
using Bivouac.Abstractions;
using Bivouac.Model;
using Burble.Abstractions;
using Burble.Abstractions.Exceptions;
using Burble.Abstractions.Extensions;
using Newtonsoft.Json;

namespace Bivouac.Services
{
   public class ApiStatusEndpointDependency : IStatusEndpointDependency
   {
      private readonly IHttpClient _httpClient;

      public ApiStatusEndpointDependency(string name, IHttpClient httpClient)
      {
         _httpClient = httpClient;
         Name = name;
      }

      public string Name { get; }

      public async Task<Status> GetStatusAsync(CancellationToken cancellationToken)
      {
         try
         {
            var response = await _httpClient.GetAsync(".status", cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var status = JsonConvert.DeserializeObject<Status>(content);
            return status;
         }
         catch (HttpClientTimeoutException)
         {
            return new Status { Name = Name, Availability = Availability.Unknown, Host = _httpClient.BaseAddress.AbsoluteUri.TrimEnd('/') };
         }
         catch (TaskCanceledException)
         {
            return new Status { Name = Name, Availability = Availability.Unknown, Host = _httpClient.BaseAddress.AbsoluteUri.TrimEnd('/') };
         }
         catch (Exception)
         {               
            return new Status { Name = Name, Availability = Availability.Unavailable, Host = _httpClient.BaseAddress.AbsoluteUri.TrimEnd('/') };
         }
      }
   }
}
