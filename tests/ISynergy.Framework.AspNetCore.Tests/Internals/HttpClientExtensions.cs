using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Tests.Internals
{
    internal static class HttpClientExtensions
    {
        internal static async Task<HttpResponseMessageWithTiming> GetWithTimingAsync(this HttpClient client, string requestUri)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await client.GetAsync(requestUri);
            var timing = stopwatch.Elapsed;

            stopwatch.Stop();

            return new HttpResponseMessageWithTiming
            {
                Response = response,
                Timing = timing
            };
        }
    }
}
