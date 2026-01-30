using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ISynergy.Framework.AspNetCore.Filters;

/// <summary>
/// Checks if HttpResponse content is null.
/// If it's null, then return a HTTP 404 (Not Found) instead of HTTP 200 (OK).
/// </summary>
public sealed class NullResultFilterAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Called when [action executed].
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var response = context.HttpContext.Response;

        if (response?.StatusCode != StatusCodes.Status200OK)
            return;
        if (!(context.Result is ObjectResult))
            return;

        if (((ObjectResult)context.Result).Value is null)
            context.Result = new NotFoundResult();
    }
}
