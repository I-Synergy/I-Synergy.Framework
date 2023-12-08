using System;
using System.Net.Http;

namespace ISynergy.Framework.AspNetCore.Tests.Internals;

/// <summary>
/// Class HttpResponseMessageWithTiming.
/// </summary>
internal class HttpResponseMessageWithTiming
{
    /// <summary>
    /// Gets or sets the response.
    /// </summary>
    /// <value>The response.</value>
    internal HttpResponseMessage Response { get; set; }
    /// <summary>
    /// Gets or sets the timing.
    /// </summary>
    /// <value>The timing.</value>
    internal TimeSpan Timing { get; set; }
}
