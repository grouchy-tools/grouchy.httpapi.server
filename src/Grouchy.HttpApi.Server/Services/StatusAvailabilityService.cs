﻿using System.Linq;
using Grouchy.HttpApi.Server.Abstractions.Model;

namespace Grouchy.HttpApi.Server.Services
{
   public class StatusAvailabilityService
   {
      public Availability GetAvailability(params Dependency[] dependencies)
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