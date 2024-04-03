using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Extensions;

public static class ExceptionExtensions
{
    /// <summary>
    /// Provides full stack trace for the exception that occurred.
    /// </summary>
    /// <param name="exception">Exception object.</param>
    /// <param name="environmentStackTrace">Environment stack trace, for pulling additional stack frames.</param>
    public static string ToMessage(this Exception exception, string environmentStackTrace)
    {
        var environmentStackTraceLines = ExceptionExtensions.GetUserStackTraceLines(environmentStackTrace);
        var stackTraceLines = ExceptionExtensions.GetStackTraceLines(exception.StackTrace);

        stackTraceLines ??= new List<string>();
        stackTraceLines.AddRange(environmentStackTraceLines);

        var fullStackTrace = string.Join(Environment.NewLine, stackTraceLines);
        var logMessage = exception.Message + Environment.NewLine + fullStackTrace;

        return logMessage;
    }

    /// <summary>
    /// Gets a list of stack frame lines, as strings.
    /// </summary>
    /// <param name="stackTrace">Stack trace string.</param>
    private static List<string> GetStackTraceLines(string stackTrace) =>
        stackTrace?.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList() ?? new List<string>();

    /// <summary>
    /// Gets a list of stack frame lines, as strings, only including those for which line number is known.
    /// </summary>
    /// <param name="fullStackTrace">Full stack trace, including external code.</param>
    private static List<string> GetUserStackTraceLines(string fullStackTrace)
    {
        var outputList = new List<string>();
        var stackTraceLines = ExceptionExtensions.GetStackTraceLines(fullStackTrace);

        foreach (string stackTraceLine in stackTraceLines.EnsureNotNull())
        {
            outputList.Add(stackTraceLine);
        }

        return outputList;
    }
}
