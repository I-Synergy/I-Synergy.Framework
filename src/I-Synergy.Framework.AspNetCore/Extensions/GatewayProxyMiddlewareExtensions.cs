using System;
using ISynergy.Framework.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    /// <summary>
    /// Class GatewayProxyMiddlewareExtensions.
    /// </summary>
    public static class GatewayProxyMiddlewareExtensions
    {
        /// <summary>
        /// Uses the gateway proxy.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>IApplicationBuilder.</returns>
        /// <exception cref="ArgumentNullException">app</exception>
        public static IApplicationBuilder UseGatewayProxy(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<GatewayProxyMiddleware>();
        }
    }
}
