using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Abstractions;
using Grouchy.Abstractions;
using Grouchy.HttpApi.Server.Abstractions.Model;
using Grouchy.HttpApi.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Grouchy.HttpApi.Server.Middleware
{
   public class StatusEndpointMiddleware
   {      
      private readonly RequestDelegate _next;
      private readonly IApplicationInfo _applicationInfo;

      public StatusEndpointMiddleware(
         RequestDelegate next,
         IApplicationInfo applicationInfo)
      {
         _next = next ?? throw new ArgumentNullException(nameof(next));
         _applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
      }

      public async Task Invoke(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         if (context.Request.Path == "/.status")
         {
            var response = new Status
            {
               Name = _applicationInfo.Name,
               Version = _applicationInfo.Version
            };

            var statusEndpointDependencies = context.RequestServices.GetServices<IStatusEndpointDependency>().ToArray();

            if (statusEndpointDependencies.Length != 0)
            {
               // Allow dependencies to complete within 1 seconds
               var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
               var statusDependencies = statusEndpointDependencies.Select(d => new { d.Name, StatusTask = d.GetStatusAsync(cancellationTokenSource.Token) }).ToArray();

               try
               {
                  // Wait for all to complete or be cancelled
                  await Task.WhenAll(statusDependencies.Select(c => c.StatusTask));
               }
               catch (TaskCanceledException)
               {
                  // Ignore cancellations
               }

               var dependencies = statusDependencies.Select(c => c.StatusTask.Status == TaskStatus.RanToCompletion ? c.StatusTask.Result : new Dependency { Name = c.Name, Availability = Availability.Unknown }).ToArray();

               var statusAvailabilityService = new StatusAvailabilityService();
               response.Availability = statusAvailabilityService.GetAvailability(dependencies);
               response.Dependencies = dependencies;
            }
            else
            {
               response.Availability = Availability.Available;
            }

            var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            context.Response.StatusCode = (int)MapStatusCode(response.Availability);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
         }
         else
         {
            await _next(context);
         }
      }

      private static HttpStatusCode MapStatusCode(Availability availability)
      {
         switch (availability)
         {
            case Availability.Available:
               return HttpStatusCode.OK;
            case Availability.Unknown:
               return HttpStatusCode.GatewayTimeout;
            default:
               return HttpStatusCode.BadGateway;
         }
      }
   }
}