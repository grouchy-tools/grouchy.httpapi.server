using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;

namespace Grouchy.HttpApi.Server.Mvc
{
   public class GuidOutputFormatter : TextOutputFormatter
   {
      public GuidOutputFormatter()
      {
         SupportedEncodings.Add(Encoding.UTF8);
         SupportedEncodings.Add(Encoding.Unicode);
         SupportedMediaTypes.Add("text/plain");
      }

      public override bool CanWriteResult(OutputFormatterCanWriteContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         if (context.ObjectType != typeof(Guid) && !(context.Object is Guid))
         {
            return false;
         }

         if (!context.ContentType.HasValue)
         {
            var mediaType = this.SupportedMediaTypes[0];
            var encoding = this.SupportedEncodings[0];
            context.ContentType = new StringSegment(MediaType.ReplaceEncoding(mediaType, encoding));
         }

         return true;
      }

      public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding encoding)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));
         if (encoding == null) throw new ArgumentNullException(nameof(encoding));

         var guid = (Guid)context.Object;

         return context.HttpContext.Response.WriteAsync(guid.ToString(), encoding, new CancellationToken());
      }
   }
}