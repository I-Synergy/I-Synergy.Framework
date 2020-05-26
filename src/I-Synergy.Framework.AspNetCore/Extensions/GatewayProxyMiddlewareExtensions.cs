using System;
using ISynergy.Framework.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    public static class GatewayProxyMiddlewareExtensions
    {
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
