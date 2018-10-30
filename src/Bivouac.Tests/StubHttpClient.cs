using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions;
using Newtonsoft.Json;

namespace Bivouac.Tests
{
   public class StubHttpClient<T> : IHttpClient
   {
      public Uri BaseAddress { get; set; }

      public Exception Exception { get; set; }

      public TimeSpan Latency { get; set; }

      public HttpStatusCode? StatusCode { get; set; }

      public T Response { get; set; }

      public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         if (Exception != null)
         {
            throw Exception;
         }

         await Task.Delay(Latency, cancellationToken);

         HttpContent content = null;

         if (Response != null)
         {
            var responseString = Response as string;
            if (responseString != null)
            {
               content = new StringContent(responseString, Encoding.UTF8, "text/plain");
            }
            else
            {
               var json = JsonConvert.SerializeObject(Response);
               content = new StringContent(json, Encoding.UTF8, "application/json");
            }
         }

         return new HttpResponseMessage(StatusCode ?? HttpStatusCode.OK) { Content = content };
      }
   }
}
