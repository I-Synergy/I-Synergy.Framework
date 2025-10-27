using System.Net;

namespace ISynergy.Framework.AspNetCore.Tests.Internals;

/// <summary>
/// Class HttpResponseMessageWithTiming.
/// </summary>
internal class HttpResponseMessageWithTiming
{
    /// <summary>
    /// Gets or sets the HTTP response.
    /// </summary>
    /// <value>The response.</value>
    internal required HttpResponseMessage Response { get; init; }

    /// <summary>
    /// Gets or sets the timing.
    /// </summary>
    /// <value>The timing.</value>
    internal TimeSpan Timing { get; init; }

    /// <summary>
    /// Gets the status code from the underlying response.
    /// </summary>
    internal HttpStatusCode StatusCode => Response.StatusCode;

    /// <summary>
    /// Gets whether the response was successful from the underlying response.
    /// </summary>
    internal bool IsSuccessStatusCode => Response.IsSuccessStatusCode;

    /// <summary>
    /// Gets the content from the underlying response.
    /// </summary>
    internal HttpContent Content => Response.Content;
}
