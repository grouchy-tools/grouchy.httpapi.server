namespace Bivouac.Middleware
{
   using System;
   using System.Linq;
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;
   using Bivouac.Abstractions;
   using Bivouac.Model;
   using Microsoft.AspNetCore.Http;
   using Microsoft.Extensions.DependencyInjection;
   using Newtonsoft.Json;
   using Newtonsoft.Json.Serialization;

   public class StatusEndpointMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly IStatusEndpointService _statusEndpointService;
      private readonly IStatusAvailabilityService _statusAvailabilityService;

      public StatusEndpointMiddleware(
         RequestDelegate next,
         IStatusEndpointService statusEndpointService,
         IStatusAvailabilityService statusAvailabilityService)
      {
         if (next == null) throw new ArgumentNullException(nameof(next));
         if (statusEndpointService == null) throw new ArgumentNullException(nameof(statusEndpointService));
         if (statusAvailabilityService == null) throw new ArgumentNullException(nameof(statusAvailabilityService));

         _next = next;
         _statusEndpointService = statusEndpointService;
         _statusAvailabilityService = statusAvailabilityService;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         if (context.Request.Path == "/status")
         {
            var response = new Status
            {
               Name = _statusEndpointService.GetName(),
               Version = _statusEndpointService.GetVersion(),
               Build = _statusEndpointService.GetBuild()
            };

            var statusEndpointDependencies = context.RequestServices.GetServices<IStatusEndpointDependency>().ToArray();

            if (statusEndpointDependencies.Length != 0)
            {
               // Allow dependencies to complete within 3 seconds
               var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
               var statusDependencies = statusEndpointDependencies.Select(d => new { d.Name, StatusTask = d.GetStatus(cancellationTokenSource.Token) }).ToArray();

               try
               {
                  // Wait for all to complete or be cancelled
                  await Task.WhenAll(statusDependencies.Select(c => c.StatusTask));
               }
               catch (TaskCanceledException)
               {
                  // Ignore cancellations
               }

               var dependencies = statusDependencies.Select(c => c.StatusTask.Status == TaskStatus.RanToCompletion ? c.StatusTask.Result : new Status { Name = c.Name, Availability = Availability.Unknown }).ToArray();

               response.Availability = _statusAvailabilityService.GetAvailability(dependencies);
               response.Dependencies = dependencies;
            }
            else
            {
               response.Availability = Availability.Available;
            }

            var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
         }
         else
         {
            await _next(context);
         }
      }
   }
}