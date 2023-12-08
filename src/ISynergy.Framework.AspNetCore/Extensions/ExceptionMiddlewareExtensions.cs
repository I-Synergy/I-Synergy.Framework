using ISynergy.Framework.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ISynergy.Framework.AspNetCore.Extensions;

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
