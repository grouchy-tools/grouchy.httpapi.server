using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Grouchy.HttpApi.Server.Mvc
{
   // TODO: Not sure we should set whole body back
   public class CreatedContentResult : ActionResult
   {
      private readonly string _location;
      private readonly object _content;

      public CreatedContentResult(string location, object content)
      {
         _location = location;
         _content = content;
      }

      public override void ExecuteResult(ActionContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         var response = context.HttpContext.Response;

         // TODO: Handle more Accept-Types

         response.StatusCode = (int)HttpStatusCode.Created;
         response.ContentType = "application/json";
         response.Headers["Location"] = _location;

         var s = new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() };

         using (var sr = new StreamWriter(response.Body))
         {
            s.Serialize(sr, _content);
         }
      }
   }
}