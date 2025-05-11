using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.ObjectPool;
using System.Collections;
using System.Text;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Extension methods for Exception objects.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Defines the verbosity level for exception messages
    /// </summary>
    public enum ExceptionVerbosityLevel
    {
        /// <summary>
        /// Minimal information (type and message only)
        /// </summary>
        Minimal,

        /// <summary>
        /// Standard information (includes inner exceptions)
        /// </summary>
        Standard,

        /// <summary>
        /// Full information (includes stack trace and all details)
        /// </summary>
        Full
    }

    // StringBuilder pool to reduce allocations
    private static readonly ObjectPool<StringBuilder> _builderPool =
        new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());

    /// <summary>
    /// Provides full stack trace for the exception that occurred.
    /// </summary>
    /// <param name="exception">Exception object.</param>
    /// <param name="environmentStackTrace">Environment stack trace, for pulling additional stack frames.</param>
    /// <param name="verbosity">The level of detail to include in the output.</param>
    /// <param name="includeEnvironmentStack">Whether to include the environment stack trace.</param>
    /// <returns>Formatted exception message with stack trace</returns>
    public static string ToMessage(
        this Exception exception,
        string environmentStackTrace,
        ExceptionVerbosityLevel verbosity = ExceptionVerbosityLevel.Full,
        bool includeEnvironmentStack = true)
    {
        if (exception == null)
            return string.Empty;

        var sb = _builderPool.Get();
        try
        {
            // Add exception type and message
            sb.AppendLine($"Exception type: {exception.GetType().FullName}");
            sb.AppendLine($"Message: {exception.Message}");

            if (verbosity >= ExceptionVerbosityLevel.Standard)
            {
                // Add exception data if available
                if (exception.Data.Count > 0)
                {
                    sb.AppendLine("Exception Data:");
                    foreach (DictionaryEntry entry in exception.Data)
                    {
                        sb.AppendLine($"  {entry.Key}: {entry.Value}");
                    }
                }

                // Add specialized exception information
                AppendSpecializedExceptionInfo(sb, exception);

                // Handle AggregateException specially
                if (exception is AggregateException aggregateException)
                {
                    AppendAggregateException(sb, aggregateException, verbosity);
                }
                else if (exception is not null && exception.InnerException is not null)
                {
                    // Add inner exception details with recursion limit
                    AppendInnerExceptions(sb, exception.InnerException, verbosity);
                }
            }

            if (verbosity == ExceptionVerbosityLevel.Full && exception is not null)
            {
                // Add stack trace information
                var stackTraceLines = GetStackTraceLines(exception.StackTrace ?? string.Empty);

                if (includeEnvironmentStack && !string.IsNullOrEmpty(environmentStackTrace))
                {
                    var environmentStackTraceLines = GetEnvironmentStackTraceLines(environmentStackTrace);
                    stackTraceLines ??= new List<string>();
                    stackTraceLines.AddRange(environmentStackTraceLines);
                }

                if (stackTraceLines?.Count > 0)
                {
                    var fullStackTrace = string.Join(Environment.NewLine, stackTraceLines);
                    sb.AppendLine("Stack trace:");
                    sb.AppendLine(fullStackTrace);
                }
            }

            return sb.ToString();
        }
        finally
        {
            sb.Clear();
            _builderPool.Return(sb);
        }
    }

    /// <summary>
    /// Provides full stack trace for the exception that occurred (backward compatibility).
    /// </summary>
    /// <param name="exception">Exception object.</param>
    /// <param name="environmentStackTrace">Environment stack trace, for pulling additional stack frames.</param>
    /// <returns>Formatted exception message with stack trace</returns>
    public static string ToMessage(this Exception exception, string environmentStackTrace)
    {
        return ToMessage(exception, environmentStackTrace, ExceptionVerbosityLevel.Full, true);
    }

    /// <summary>
    /// Adds specialized information for specific exception types.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <param name="exception">The exception to process.</param>
    private static void AppendSpecializedExceptionInfo(StringBuilder sb, Exception exception)
    {
        if (exception is HttpRequestException httpEx)
        {
            sb.AppendLine($"Status Code: {httpEx.StatusCode}");
        }

        // Special handling for StackOverflowException
        // Note: This won't catch StackOverflowException at runtime, but can help with post-mortem analysis
        // when the exception type is known but was not caught (e.g., from logs or crash reports)
        else if (exception is StackOverflowException)
        {
            sb.AppendLine("WARNING: StackOverflowException detected - this typically indicates infinite recursion or extremely deep call stacks");
            sb.AppendLine("This exception cannot be caught at runtime and will terminate the process");
            sb.AppendLine("HResult: 0x800703E9");
        }
    }

    /// <summary>
    /// Appends inner exception details with a recursion limit.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <param name="innerException">The inner exception to process.</param>
    /// <param name="verbosity">The level of detail to include.</param>
    /// <param name="currentDepth">Current recursion depth.</param>
    /// <param name="maxDepth">Maximum recursion depth.</param>
    private static void AppendInnerExceptions(
        StringBuilder sb,
        Exception innerException,
        ExceptionVerbosityLevel verbosity,
        int currentDepth = 0,
        int maxDepth = 10)
    {
        if (innerException == null)
            return;

        if (currentDepth >= maxDepth)
        {
            sb.AppendLine("(Maximum recursion depth reached for inner exceptions)");
            return;
        }

        sb.AppendLine($"Inner exception: {innerException.GetType().FullName}");
        sb.AppendLine($"Message: {innerException.Message}");

        if (verbosity == ExceptionVerbosityLevel.Full)
        {
            if (innerException.Data.Count > 0)
            {
                sb.AppendLine("Exception Data:");
                foreach (DictionaryEntry entry in innerException.Data)
                {
                    sb.AppendLine($"  {entry.Key}: {entry.Value}");
                }
            }
        }

        if (innerException is not null && innerException.InnerException is not null)
            AppendInnerExceptions(sb, innerException.InnerException, verbosity, currentDepth + 1, maxDepth);
    }

    /// <summary>
    /// Gets a list of stack frame lines, as strings.
    /// </summary>
    /// <param name="stackTrace">Stack trace string.</param>
    private static List<string> GetStackTraceLines(string stackTrace) =>
        stackTrace?.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList() ?? new List<string>();

    /// <summary>
    /// Gets a list of stack frame lines from the environment stack trace.
    /// </summary>
    /// <param name="fullStackTrace">Full stack trace, including external code.</param>
    private static List<string> GetEnvironmentStackTraceLines(string fullStackTrace)
    {
        if (string.IsNullOrEmpty(fullStackTrace))
            return new List<string>();

        return GetStackTraceLines(fullStackTrace);
    }

    /// <summary>
    /// Appends information about an AggregateException to the StringBuilder.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <param name="aggregateException">The AggregateException to process.</param>
    /// <param name="verbosity">The level of detail to include.</param>
    private static void AppendAggregateException(
        StringBuilder sb,
        AggregateException aggregateException,
        ExceptionVerbosityLevel verbosity)
    {
        sb.AppendLine($"AggregateException with {aggregateException.InnerExceptions.Count} inner exceptions:");

        for (int i = 0; i < aggregateException.InnerExceptions.Count; i++)
        {
            var innerEx = aggregateException.InnerExceptions[i];
            sb.AppendLine($"--- Inner Exception {i + 1} ---");
            sb.AppendLine($"Type: {innerEx.GetType().FullName}");
            sb.AppendLine($"Message: {innerEx.Message}");

            if (verbosity == ExceptionVerbosityLevel.Full && !string.IsNullOrEmpty(innerEx.StackTrace))
            {
                sb.AppendLine("Stack trace:");
                sb.AppendLine(innerEx.StackTrace);
            }
        }
    }
}
