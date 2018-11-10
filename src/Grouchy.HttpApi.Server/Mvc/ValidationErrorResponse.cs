namespace Grouchy.HttpApi.Server.Mvc
{
   public class ValidationErrorResponse
   {
      public string Type { get; set; } = "ValidationError";

      public ValidationError[] Errors { get; set; }
   }
}