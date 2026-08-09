using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.ObjectPool;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable S3776 // cognitive complexity is inherent in exception formatting logic
#pragma warning disable S3427 // 2-param overload is intentional backward-compatible wrapper; default params in full overload are by design

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Extension methods for Exception objects.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Defines the verbosity level for exception messages
    /// </summary>
    public enum ExceptionVerbosityLevels
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

    private static readonly TimeSpan _regexTimeout = TimeSpan.FromMilliseconds(100);

    // Maximum input length for regex sanitization to prevent catastrophic backtracking
    // on very large inputs like full stack traces with embedded connection strings.
    private const int MaxSanitizeInputLength = 64_000;

    // Patterns for sensitive data that should be sanitized.
    // IMPORTANT: all quantifiers must be bounded to avoid runaway backtracking.
    private static readonly Regex[] _sensitivePatterns = new[]
    {
        // Connection strings — keywords followed by = value.
        // \b prevents partial matches (e.g. "ServerName" matching "Server").
        // {1,300} upper bound on the value avoids quadratic scan on long lines.
        new Regex(@"(?i)\b(ConnectionString|Connection\s+String|Data\s+Source|Server|Initial\s+Catalog|Database|User\s+ID|User\s+Id|UID|Pwd|Password|Integrated\s+Security|Trusted_Connection)\s*=\s*[^;,\s]{1,300}", RegexOptions.Compiled | RegexOptions.IgnoreCase, _regexTimeout),
        // Passwords and tokens — key:value or key=value pairs.
        // Longer alternatives come first so "password" is tried before the prefix "pass".
        // \b boundaries and {1,300} upper bound prevent runaway scans.
        new Regex(@"(?i)\b(password|passwd|accesstoken|pass|token|bearer|apikey|secret|pwd|key)\s*[:=]\s*['""]?[^'""\s]{1,300}['""]?", RegexOptions.Compiled | RegexOptions.IgnoreCase, _regexTimeout),
        // API keys and secrets — base64-looking values.
        // {20,500} upper bound prevents scanning indefinitely through long base64 blobs.
        new Regex(@"(?i)\b(api[_-]?key|secret[_-]?key|private[_-]?key|public[_-]?key)\s*[:=]\s*['""]?[A-Za-z0-9+/=]{20,500}['""]?", RegexOptions.Compiled | RegexOptions.IgnoreCase, _regexTimeout),
        // JWT tokens — three base64url segments separated by dots.
        // {10,200} upper bound per segment prevents catastrophic backtracking when
        // "eyJ" appears but no valid JWT follows (e.g. in long stack traces).
        new Regex(@"(?i)\b(eyJ[A-Za-z0-9_-]{10,200}\.[A-Za-z0-9_-]{10,200}\.[A-Za-z0-9_-]{10,200})", RegexOptions.Compiled, _regexTimeout),
        // File paths with usernames (Windows)
        new Regex(@"(?i)(C:\\Users\\)[^\\]{1,200}", RegexOptions.Compiled, _regexTimeout),
        // File paths with usernames (Unix)
        new Regex(@"(?i)(/home/|/Users/)[^/]{1,200}", RegexOptions.Compiled, _regexTimeout),
    };

    private const string SanitizedValue = "[REDACTED]";

    /// <summary>
    /// Sanitizes a string by removing or masking sensitive information.
    /// </summary>
    /// <param name="input">The input string to sanitize.</param>
    /// <returns>Sanitized string with sensitive data replaced.</returns>
    public static string SanitizeSensitiveData(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Skip regex sanitization for very large inputs to prevent timeout.
        // Exception messages with full stack traces can exceed this threshold,
        // and running unbounded regex over them risks RegexMatchTimeoutException.
        if (input.Length > MaxSanitizeInputLength)
            return input;

        var sanitized = input;
        foreach (var pattern in _sensitivePatterns)
        {
            try
            {
                sanitized = pattern.Replace(sanitized, match =>
                {
                    // Preserve the key part but redact the value
                    var matchValue = match.Value;
                    var colonOrEqualsIndex = matchValue.IndexOfAny(new[] { ':', '=' });
                    if (colonOrEqualsIndex > 0 && colonOrEqualsIndex < matchValue.Length - 1)
                    {
                        return matchValue.Substring(0, colonOrEqualsIndex + 1) + " " + SanitizedValue;
                    }
                    return SanitizedValue;
                });
            }
            catch (RegexMatchTimeoutException)
            {
                // If the pattern times out on a very long/complex input, skip it and continue.
                // Returning the partially-sanitized string is safer than propagating the exception.
            }
        }

        return sanitized;
    }

    /// <summary>
    /// Provides full stack trace for the exception that occurred.
    /// </summary>
    /// <param name="exception">Exception object.</param>
    /// <param name="environmentStackTrace">Environment stack trace, for pulling additional stack frames.</param>
    /// <param name="verbosity">The level of detail to include in the output.</param>
    /// <param name="includeEnvironmentStack">Whether to include the environment stack trace.</param>
    /// <param name="sanitizeSensitiveData">Whether to sanitize sensitive data from the output.</param>
    /// <returns>Formatted exception message with stack trace</returns>
    public static string ToMessage( // NOSONAR
        this Exception exception,
        string environmentStackTrace,
        ExceptionVerbosityLevels verbosity = ExceptionVerbosityLevels.Full,
        bool includeEnvironmentStack = true,
        bool sanitizeSensitiveData = true)
    {
        if (exception == null)
            return string.Empty;

        var sb = _builderPool.Get();
        try
        {
            // Add exception type and message
            var exceptionMessage = sanitizeSensitiveData ? SanitizeSensitiveData(exception.Message) : exception.Message;
            sb.AppendLine($"Exception type: {exception.GetType().FullName}");
            sb.AppendLine($"Message: {exceptionMessage}");

            if (verbosity >= ExceptionVerbosityLevels.Standard)
            {
                // Add exception data if available
                if (exception.Data.Count > 0)
                {
                    sb.AppendLine("Exception Data:");
                    foreach (DictionaryEntry entry in exception.Data)
                    {
                        var value = entry.Value?.ToString() ?? string.Empty;
                        if (sanitizeSensitiveData)
                            value = SanitizeSensitiveData(value);
                        sb.AppendLine($"  {entry.Key}: {value}");
                    }
                }

                // Add specialized exception information
                AppendSpecializedExceptionInfo(sb, exception);

                // Handle AggregateException specially
                if (exception is AggregateException aggregateException)
                {
                    AppendAggregateException(sb, aggregateException, verbosity, sanitizeSensitiveData);
                }
                else if (exception.InnerException is not null)
                {
                    // Add inner exception details with recursion limit
                    AppendInnerExceptions(sb, exception.InnerException, verbosity, sanitizeSensitiveData);
                }
            }

            if (verbosity == ExceptionVerbosityLevels.Full)
            {
                // Add stack trace information
                var stackTrace = exception.StackTrace ?? string.Empty;
                if (sanitizeSensitiveData)
                    stackTrace = SanitizeSensitiveData(stackTrace);
                
                var stackTraceLines = GetStackTraceLines(stackTrace);

                if (includeEnvironmentStack && !string.IsNullOrEmpty(environmentStackTrace))
                {
                    var envStackTrace = sanitizeSensitiveData ? SanitizeSensitiveData(environmentStackTrace) : environmentStackTrace;
                    var environmentStackTraceLines = GetEnvironmentStackTraceLines(envStackTrace);
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

            var result = sb.ToString();
            return sanitizeSensitiveData ? SanitizeSensitiveData(result) : result;
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
        return ToMessage(exception, environmentStackTrace, ExceptionVerbosityLevels.Full, true, true);
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
    /// <param name="sanitizeSensitiveData">Whether to sanitize sensitive data.</param>
    /// <param name="currentDepth">Current recursion depth.</param>
    /// <param name="maxDepth">Maximum recursion depth.</param>
    private static void AppendInnerExceptions( // NOSONAR
        StringBuilder sb,
        Exception innerException,
        ExceptionVerbosityLevels verbosity,
        bool sanitizeSensitiveData = true,
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

        var innerMessage = sanitizeSensitiveData ? SanitizeSensitiveData(innerException.Message) : innerException.Message;
        sb.AppendLine($"Inner exception: {innerException.GetType().FullName}");
        sb.AppendLine($"Message: {innerMessage}");

        if (verbosity == ExceptionVerbosityLevels.Full && innerException.Data.Count > 0)
        {
            sb.AppendLine("Exception Data:");
            foreach (DictionaryEntry entry in innerException.Data)
            {
                var value = entry.Value?.ToString() ?? string.Empty;
                if (sanitizeSensitiveData)
                    value = SanitizeSensitiveData(value);
                sb.AppendLine($"  {entry.Key}: {value}");
            }
        }

        if (innerException.InnerException is not null)
            AppendInnerExceptions(sb, innerException.InnerException, verbosity, sanitizeSensitiveData, currentDepth + 1, maxDepth);
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
    /// <param name="sanitizeSensitiveData">Whether to sanitize sensitive data.</param>
    private static void AppendAggregateException(
        StringBuilder sb,
        AggregateException aggregateException,
        ExceptionVerbosityLevels verbosity,
        bool sanitizeSensitiveData = true)
    {
        sb.AppendLine($"AggregateException with {aggregateException.InnerExceptions.Count} inner exceptions:");

        for (int i = 0; i < aggregateException.InnerExceptions.Count; i++)
        {
            var innerEx = aggregateException.InnerExceptions[i];
            var innerMessage = sanitizeSensitiveData ? SanitizeSensitiveData(innerEx.Message) : innerEx.Message;
            sb.AppendLine($"--- Inner Exception {i + 1} ---");
            sb.AppendLine($"Type: {innerEx.GetType().FullName}");
            sb.AppendLine($"Message: {innerMessage}");

            if (verbosity == ExceptionVerbosityLevels.Full && !string.IsNullOrEmpty(innerEx.StackTrace))
            {
                var stackTrace = sanitizeSensitiveData ? SanitizeSensitiveData(innerEx.StackTrace) : innerEx.StackTrace;
                sb.AppendLine("Stack trace:");
                sb.AppendLine(stackTrace);
            }
        }
    }
}
