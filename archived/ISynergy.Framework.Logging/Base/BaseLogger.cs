using ISynergy.Framework.Logging.Providers;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Logging.Base;

public class BaseLogger : ILogger, IDisposable
{
    protected readonly LoggingContextProvider _contextProvider;
    protected readonly ILogger _logger;

    protected BaseLogger(ILoggerFactory loggerFactory, LoggingContextProvider contextProvider, string name)
    {
        _logger = loggerFactory.CreateLogger(name);
        _contextProvider = contextProvider;

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Flush();
    }

    public virtual IDisposable BeginScope<TState>(TState state) where TState : notnull => _logger.BeginScope(state);

    public void AddCustomAttribute(string key, object value, bool isGlobal = false)
    {
        if (isGlobal)
            _contextProvider.SetGlobalAttribute(key, value);
        else
            _contextProvider.SetContextAttribute(key, value);
    }

    /// <summary>
    /// If the filter is null, everything is enabled unless the debugger is not attached
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public virtual bool IsEnabled(LogLevel logLevel) =>
        logLevel != LogLevel.None;

    public virtual void Flush()
    {
        try
        {
            // If there's any pending logging operations, allow them to complete
            // with a reasonable timeout
        }
        catch (OperationCanceledException)
        {
            // Silently ignore task cancellations during shutdown
        }
        catch (Exception ex)
        {
            // Optionally log this to console or debug output
            System.Diagnostics.Debug.WriteLine($"Error during logger flush: {ex.Message}");
        }
    }

    // Add a Dispose method to properly clean up resources
    public virtual void Dispose()
    {
        try
        {
            Flush();

            // If _logger implements IDisposable, dispose it
            if (_logger is IDisposable disposableLogger)
            {
                disposableLogger.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
            // Silently ignore task cancellations during disposal
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during logger disposal: {ex.Message}");
        }
    }

    //public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    //{
    //    var message = formatter(state, exception);

    //    if (string.IsNullOrEmpty(message))
    //        return;

    //    using var scope = _logger.BeginScope(_customAttributes);

    //    switch (logLevel)
    //    {
    //        case LogLevel.Error:
    //        case LogLevel.Critical:
    //            _logger.LogError(exception, message);
    //            break;
    //        case LogLevel.Debug:
    //        case LogLevel.Trace:
    //            if (message.EndsWith("ViewModel") || message.EndsWith("View") || message.EndsWith("Window"))
    //            {
    //                AddCustomAttribute("page.name", message.Replace("ViewModel", "")
    //                    .Replace("View", "").Replace("Window", ""));
    //            }
    //            _logger.LogDebug(message);
    //            break;
    //        default:
    //            _logger.LogInformation(message);
    //            break;
    //    }
    //}

    public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        // Get all combined attributes
        var allAttributes = _contextProvider.GetAllAttributes();

        // Use them in the scope
        using var scope = _logger.BeginScope(allAttributes);

        // Forward the log call with original parameters to preserve structured logging
        _logger.Log(logLevel, eventId, state, exception, formatter);
    }
}
