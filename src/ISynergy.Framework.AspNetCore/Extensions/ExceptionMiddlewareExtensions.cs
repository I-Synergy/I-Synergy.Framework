using Microsoft.AspNetCore.Builder;
using ISynergy.Framework.AspNetCore.Middleware;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    /// <summary>
    /// Class ExceptionMiddlewareExtensions.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Configures the exception handler middleware.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void ConfigureExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
