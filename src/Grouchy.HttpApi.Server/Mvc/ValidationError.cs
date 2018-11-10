namespace Grouchy.HttpApi.Server.Mvc
{
   public abstract class ValidationError
   {
      public string Message { get; set; }

      public string Field { get; set; }
   }
}