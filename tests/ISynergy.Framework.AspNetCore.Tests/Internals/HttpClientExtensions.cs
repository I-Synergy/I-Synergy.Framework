﻿using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Tests.Internals
{
    /// <summary>
    /// Class HttpClientExtensions.
    /// </summary>
    internal static class HttpClientExtensions
    {
        /// <summary>
        /// get with timing as an asynchronous operation.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="requestUri">The request URI.</param>
        /// <returns>A Task&lt;HttpResponseMessageWithTiming&gt; representing the asynchronous operation.</returns>
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
