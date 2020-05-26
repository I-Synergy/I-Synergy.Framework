using System;
using ISynergy.Framework.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    public static class MaxConcurrentRequestsMiddlewareExtensions
    {
        public static IApplicationBuilder UseMaxConcurrentRequests(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<MaxConcurrentRequestsMiddleware>();
        }
    }
}
