namespace ISynergy.Framework.AspNetCore.Tests.Internals;

/// <summary>
/// Extension methods for HttpResponseMessage.
/// </summary>
internal static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Creates a HttpResponseMessageWithTiming wrapper with the specified timing information.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="timing">The elapsed time for the request.</param>
    /// <returns>A HttpResponseMessageWithTiming instance.</returns>
    internal static HttpResponseMessageWithTiming WithTiming(
        this HttpResponseMessage response,
        TimeSpan timing)
    {
        ArgumentNullException.ThrowIfNull(response);

        return new HttpResponseMessageWithTiming
        {
            Response = response,
            Timing = timing
        };
    }
}
