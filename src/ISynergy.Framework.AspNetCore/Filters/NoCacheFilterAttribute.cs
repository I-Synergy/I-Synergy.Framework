using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace ISynergy.Framework.AspNetCore.Filters;

/// <summary>
/// Class NoCacheFilter. This class cannot be inherited.
/// Implements the <see cref="ActionFilterAttribute" />
/// </summary>
/// <seealso cref="ActionFilterAttribute" />
public sealed class NoCacheFilterAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Called when [action executed].
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Response is not null)
            context.HttpContext.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue() { NoCache = true };
    }
}
