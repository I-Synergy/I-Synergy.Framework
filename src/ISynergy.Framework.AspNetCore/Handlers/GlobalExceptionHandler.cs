using Microsoft.AspNetCore.Diagnostics;
#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Net.Mime;

namespace ISynergy.Framework.AspNetCore.Handlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, ProblemDetailsFactory problemDetailsFactory) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Client disconnected — this is expected behaviour (browser tab closed, network
        // drop, Blazor Server SignalR circuit reset, etc.).  Log at Debug so it is
        // traceable without polluting error dashboards, and return without attempting
        // to write a response to a dead connection.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug("Request cancelled — client disconnected before the response was sent.");
            return true;
        }

        logger.LogError(exception, "An unhandled exception occurred while processing the request.");

        var problemDetails = problemDetailsFactory.CreateProblemDetails(httpContext,
            statusCode: StatusCodes.Status500InternalServerError,
            detail: exception.Message);

        httpContext.Response.ContentType = MediaTypeNames.Application.ProblemJson;
        httpContext.Response.StatusCode = (int)problemDetails.Status!;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
#endif