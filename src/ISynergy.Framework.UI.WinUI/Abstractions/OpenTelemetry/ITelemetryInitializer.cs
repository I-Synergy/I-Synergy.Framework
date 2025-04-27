namespace ISynergy.Framework.UI.Abstractions.OpenTelemetry;
/// <summary>
/// Interface for WinUI telemetry initializers.
/// </summary>
public interface ITelemetryInitializer
{
    /// <summary>
    /// Initializes telemetry for a WinUI window.
    /// </summary>
    /// <param name="window">The window to initialize telemetry for.</param>
    void Initialize(Microsoft.UI.Xaml.Window window);
}
