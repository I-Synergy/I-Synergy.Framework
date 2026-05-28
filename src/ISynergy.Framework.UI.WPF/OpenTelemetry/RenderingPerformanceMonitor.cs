using ISynergy.Framework.Core.Validation;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Windows.Media;

namespace ISynergy.Framework.UI.OpenTelemetry;
/// <summary>
/// Monitors WPF rendering performance and reports metrics.
/// </summary>
public class RenderingPerformanceMonitor
{
    private readonly ILogger<RenderingPerformanceMonitor> _logger;
    private readonly ApplicationMonitor _applicationMonitor;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    /// <summary>
    /// Initializes a new instance of the RenderingPerformanceMonitor class.
    /// </summary>
    /// <param name="applicationMonitor">The application monitor.</param>
    /// <param name="logger"></param>
    public RenderingPerformanceMonitor(
        ApplicationMonitor applicationMonitor,
        ILogger<RenderingPerformanceMonitor> logger)
    {
        _applicationMonitor = Argument.IsNotNull(applicationMonitor);
        _logger = Argument.IsNotNull(logger);

        // Hook rendering events
        CompositionTarget.Rendering += OnRendering;
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        if (!_stopwatch.IsRunning)
        {
            _stopwatch.Start();
            return;
        }

        _stopwatch.Stop();
        var renderTimeMs = _stopwatch.Elapsed.TotalMilliseconds;

        // Only record metrics for non-trivial frames
        if (renderTimeMs > 0.1)
        {
            _applicationMonitor.RecordRenderTime(renderTimeMs);

            // Log slow frames
            if (renderTimeMs > 16.7) // More than 60fps threshold
                _logger.LogTrace("Slow rendering frame detected: {RenderTimeMs}ms", renderTimeMs);
        }

        // Reset for next frame
        _stopwatch.Reset();
        _stopwatch.Start();
    }

    /// <summary>
    /// Attaches the monitor to the specified window.
    /// </summary>
    /// <param name="window">The window to monitor.</param>
    public void AttachToWindow(System.Windows.Window window)
    {
        Argument.IsNotNull(window);

        var layoutUpdatedStopwatch = new Stopwatch();

        window.LayoutUpdated += (s, e) =>
        {
            if (!layoutUpdatedStopwatch.IsRunning)
            {
                layoutUpdatedStopwatch.Start();
                return;
            }

            layoutUpdatedStopwatch.Stop();

            var layoutTimeMs = layoutUpdatedStopwatch.Elapsed.TotalMilliseconds;

            if (layoutTimeMs > 0.1)
                _applicationMonitor.RecordLayoutTime(layoutTimeMs);

            layoutUpdatedStopwatch.Reset();
            layoutUpdatedStopwatch.Start();
        };
    }
}