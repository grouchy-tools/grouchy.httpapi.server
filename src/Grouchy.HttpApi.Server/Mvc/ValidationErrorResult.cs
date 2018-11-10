using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Grouchy.HttpApi.Server.Mvc
{
   public class ValidationErrorResult : ActionResult
   {
      private readonly ValidationError[] _errors;

      public ValidationErrorResult(params ValidationError[] errors)
      {
         _errors = errors;
      }

      public override void ExecuteResult(ActionContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         var response = context.HttpContext.Response;

         response.StatusCode = (int)HttpStatusCode.BadRequest;
         response.ContentType = "application/json";

         var s = new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() };

         using (var sr = new StreamWriter(response.Body))
         {
            s.Serialize(sr, new ValidationErrorResponse { Errors = _errors });
         }
      }
   }
}