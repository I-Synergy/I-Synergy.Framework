using ISynergy.Framework.Core.Validation;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;

namespace Sample.OpenTelemetry;
/// <summary>
/// Monitors WinUI application metrics and performance.
/// </summary>
public class ApplicationMonitor : IDisposable
{
    private readonly ILogger<ApplicationMonitor> _logger;
    private readonly Meter _meter;
    private readonly Histogram<double> _renderTimeHistogram;
    private readonly Histogram<double> _layoutTimeHistogram;
    private readonly Timer _performanceTimer;
    private readonly Process _currentProcess;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the WinUIApplicationMonitor class.
    /// </summary>
    /// <param name="logger"></param>
    public ApplicationMonitor(ILogger<ApplicationMonitor> logger)
    {
        _logger = Argument.IsNotNull(logger);

        var appName = "WinUIApplication";
        try
        {
            appName = Windows.ApplicationModel.Package.Current?.DisplayName ?? appName;
        }
        catch (COMException)
        {
            // Ignore package access errors in non-packaged apps
        }

        _meter = new Meter($"{appName}.UI.Metrics");

        // Create performance metrics
        _renderTimeHistogram = _meter.CreateHistogram<double>(
            "winui.render.time",
            "ms",
            "Time spent rendering");

        _layoutTimeHistogram = _meter.CreateHistogram<double>(
            "winui.layout.time",
            "ms",
            "Time spent in layout");

        Sdk.CreateMeterProviderBuilder().AddInstrumentation<Meter>(() => _meter);

        // Setup periodic performance monitoring
        _currentProcess = Process.GetCurrentProcess();
        _performanceTimer = new Timer(CollectPerformanceMetrics, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
    }

    private void CollectPerformanceMetrics(object? state)
    {
        try
        {
            // Refresh process info to get current values
            _currentProcess.Refresh();

            // Collect and report metrics
            var memoryMegabytes = _currentProcess.WorkingSet64 / (1024.0 * 1024.0);

            var tags = new TagList
                {
                    { "process.name", _currentProcess.ProcessName },
                    { "process.id", _currentProcess.Id }
                };

            // Monitor memory usage
            _meter.CreateObservableGauge("winui.memory.usage.mb", () => memoryMegabytes, "MB", "Memory usage");

            // Monitor CPU usage
            _meter.CreateObservableGauge("winui.cpu.usage.percent", () => _currentProcess.TotalProcessorTime.TotalMilliseconds, "%", "CPU usage");

            // Monitor thread count
            _meter.CreateObservableGauge("winui.threads.count", () => _currentProcess.Threads.Count, "Threads", "Thread count");
        }
        catch (Exception ex)
        {
            // Log internally but don't crash the application
            _logger.LogError(ex, "Error collecting performance metrics");
        }
    }

    /// <summary>
    /// Records the time spent in a render operation.
    /// </summary>
    /// <param name="milliseconds">The time in milliseconds.</param>
    public void RecordRenderTime(double milliseconds)
    {
        _renderTimeHistogram.Record(milliseconds);
    }

    /// <summary>
    /// Records the time spent in a layout operation.
    /// </summary>
    /// <param name="milliseconds">The time in milliseconds.</param>
    public void RecordLayoutTime(double milliseconds)
    {
        _layoutTimeHistogram.Record(milliseconds);
    }

    /// <summary>
    /// Disposes the monitor and stops collecting metrics.
    /// </summary>
    public void Dispose()
    {
        if (!_isDisposed)
        {
            _performanceTimer?.Dispose();
            _meter?.Dispose();
            _isDisposed = true;
        }
    }
}