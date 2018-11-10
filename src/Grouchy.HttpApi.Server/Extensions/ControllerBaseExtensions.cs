using Grouchy.HttpApi.Server.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Grouchy.HttpApi.Server.Extensions
{
   public static class ControllerBaseExtensions
   {
      public static CreatedContentResult CreatedContent(this ControllerBase controller, string location, object content)
      {
         return new CreatedContentResult(location, content);
      }
   }
}