using ISynergy.Framework.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Logging.Base;

public class BaseLogger : ILogger, IDisposable
{
    protected readonly Dictionary<string, object> _customAttributes = new();

    protected readonly ILogger _logger;

    protected BaseLogger(string name, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(name);

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Flush();
    }

    public virtual IDisposable BeginScope<TState>(TState state) where TState : notnull =>
        NullScope.Instance;

    public void AddCustomAttribute(string key, object value)
    {
        _customAttributes[key] = value;
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

            // Clear any custom attributes
            _customAttributes.Clear();

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

    public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);

        if (string.IsNullOrEmpty(message))
            return;

        using var scope = _logger.BeginScope(_customAttributes);

        switch (logLevel)
        {
            case LogLevel.Error:
            case LogLevel.Critical:
                _logger.LogError(exception, message);
                break;
            case LogLevel.Debug:
            case LogLevel.Trace:
                if (message.EndsWith("ViewModel") || message.EndsWith("View") || message.EndsWith("Window"))
                {
                    AddCustomAttribute("page.name", message.Replace("ViewModel", "")
                        .Replace("View", "").Replace("Window", ""));
                }
                _logger.LogDebug(message);
                break;
            default:
                _logger.LogInformation(message);
                break;
        }
    }
}
