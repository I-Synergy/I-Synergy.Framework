using System;
using ISynergy.Framework.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    /// <summary>
    /// Class MaxConcurrentRequestsMiddlewareExtensions.
    /// </summary>
    public static class MaxConcurrentRequestsMiddlewareExtensions
    {
        /// <summary>
        /// Uses the maximum concurrent requests.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>IApplicationBuilder.</returns>
        /// <exception cref="ArgumentNullException">app</exception>
        public static IApplicationBuilder UseMaxConcurrentRequests(this IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<MaxConcurrentRequestsMiddleware>();
        }
    }
}
