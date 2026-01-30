using ISynergy.Framework.Core.Validation;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml.Media;
using System.Diagnostics;

namespace Sample.OpenTelemetry;

internal class RenderingPerformanceMonitor
{
    private readonly ILogger<RenderingPerformanceMonitor> _logger;
    private readonly ApplicationMonitor _applicationMonitor;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    /// <summary>
    /// Initializes a new instance of the WinUIApplicationMonitor class.
    /// </summary>
    /// <param name="applicationMonitor"></param>
    /// <param name="logger"></param>
    public RenderingPerformanceMonitor(
        ApplicationMonitor applicationMonitor,
        ILogger<RenderingPerformanceMonitor> logger)
    {
        _applicationMonitor = Argument.IsNotNull(applicationMonitor);
        _logger = Argument.IsNotNull(logger);

        // Setup rendering monitoring
        CompositionTarget.Rendering += OnRendering;
    }

    private void OnRendering(object? sender, object e)
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
}
