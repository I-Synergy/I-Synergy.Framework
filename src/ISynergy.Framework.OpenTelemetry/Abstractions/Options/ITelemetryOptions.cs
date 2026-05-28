namespace ISynergy.Framework.OpenTelemetry.Abstractions.Options;
public interface ITelemetryOptions
{
    /// <summary>
    /// Gets or sets the environment.
    /// </summary>
    string? Environment { get; set; }

    /// <summary>
    /// Gets the collection of additional attributes to include with telemetry.
    /// </summary>
    Dictionary<string, string> Attributes { get; }

    /// <summary>
    /// Gets the collection of activity source names to monitor.
    /// </summary>
    List<string> ActivitySources { get; }

    /// <summary>
    /// Gets the collection of meter names to monitor.
    /// </summary>
    List<string> MeterNames { get; }
}
