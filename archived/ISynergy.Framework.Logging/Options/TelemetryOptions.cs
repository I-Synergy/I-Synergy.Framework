namespace ISynergy.Framework.Logging.Options;
/// <summary>
/// Contains common configuration options for telemetry providers.
/// </summary>
public class TelemetryOptions
{
    /// <summary>
    /// The service name to identify this application in telemetry data.
    /// </summary>
    public string ServiceName { get; set; } = "UnnamedService";

    /// <summary>
    /// The service version to associate with telemetry data.
    /// </summary>
    public string ServiceVersion { get; set; } = "1.0.0";

    /// <summary>
    /// The service namespace to use for categorizing telemetry data.
    /// </summary>
    public string ServiceNamespace { get; set; } = "Default";

    /// <summary>
    /// The environment in which the application is running.
    /// </summary>
    public string Environment { get; set; } = "Production";

    /// <summary>
    /// Custom attributes to add to all telemetry items.
    /// </summary>
    public Dictionary<string, string> CustomAttributes { get; set; } = new();

    /// <summary>
    /// Sample rate for traces (1.0 = 100%, 0.1 = 10%).
    /// </summary>
    public double TraceSamplingRatio { get; set; } = 1.0;
}