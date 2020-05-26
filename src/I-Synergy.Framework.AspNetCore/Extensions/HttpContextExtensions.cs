using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    public static class HttpContextExtensions
    {
        public static Task SendProxiedHttpRequestAsync(this HttpContext context, HttpResponseMessage responseMessage, ILogger logger = null)
        {
            var response = context.Response;

            response.StatusCode = (int)responseMessage.StatusCode;

            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            response.Headers.Remove("transfer-encoding");

            return responseMessage.Content.CopyToAsync(response.Body);
        }
    }
}
