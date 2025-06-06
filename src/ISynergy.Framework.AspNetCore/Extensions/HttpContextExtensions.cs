﻿using ISynergy.Framework.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.AspNetCore.Extensions;

/// <summary>
/// Class HttpContextExtensions.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Sends the proxied HTTP request asynchronous.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="responseMessage">The response message.</param>
    /// <param name="loggerFactory">The logger.</param>
    /// <returns>Task.</returns>
    public static Task SendProxiedHttpRequestAsync(this HttpContext context, HttpResponseMessage responseMessage, ILoggerFactory? loggerFactory = null)
    {
        var response = context.Response;

        response.StatusCode = (int)responseMessage.StatusCode;

        foreach (var header in responseMessage.Headers.EnsureNotNull())
            response.Headers[header.Key] = header.Value.ToArray();

        foreach (var header in responseMessage.Content.Headers.EnsureNotNull())
            response.Headers[header.Key] = header.Value.ToArray();

        response.Headers.Remove("transfer-encoding");

        return responseMessage.Content.CopyToAsync(response.Body);
    }
}
