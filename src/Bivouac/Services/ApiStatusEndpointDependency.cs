namespace Bivouac.Services
{
   using System;
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;
   using Bivouac.Abstractions;
   using Bivouac.Model;
   using Burble.Abstractions;
   using Burble;
   using Newtonsoft.Json;

   public class ApiStatusEndpointDependency : IStatusEndpointDependency
   {
      private readonly IHttpClient _httpClient;

      public ApiStatusEndpointDependency(string name, IHttpClient httpClient)
      {
         _httpClient = httpClient;
         Name = name;
      }

      public string Name { get; }

      public async Task<Status> GetStatus(CancellationToken cancellationToken)
      {
         try
         {

            var request = new HttpRequestMessage(HttpMethod.Get, "status");
            var response = await _httpClient.SendAsync(request, cancellationToken);

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

/*
      public async Task<Status> GetStatus(CancellationToken cancellationToken)
      {
         try
         {
            var response = await _httpClient.GetAsync("status", cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var status = JsonConvert.DeserializeObject<Status>(content);
            return status;
         }
         catch (HttpClientServerUnavailableException)
         {
            return new Status { Name = Name, Availability = Availability.Unavailable, Host = _httpClient.BaseAddress.AbsoluteUri.TrimEnd('/') };
         }
         catch (Exception)
         {
            return new Status { Name = Name, Availability = Availability.Unknown, Host = _httpClient.BaseAddress.AbsoluteUri.TrimEnd('/') };
         }
      }
 */
