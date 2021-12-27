using ISynergy.Framework.AspNetCore.Middleware;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Builder;
using System;

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
            Argument.IsNotNull(app);

            return app.UseMiddleware<MaxConcurrentRequestsMiddleware>();
        }
    }
}
