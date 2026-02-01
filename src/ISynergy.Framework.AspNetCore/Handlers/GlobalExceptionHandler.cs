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
        logger.LogError(exception, "Whoopsie");

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