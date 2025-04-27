using ISynergy.Framework.Logging.Abstractions.Builders;
using ISynergy.Framework.Logging.Options;

namespace ISynergy.Framework.Logging.Abstractions.Providers;

/// <summary>
/// Interface for telemetry providers.
/// </summary>
public interface ITelemetryProvider
{
    /// <summary>
    /// Configures the telemetry provider with the specified service collection and options.
    /// </summary>
    void Configure(ITelemetryBuilder builder, TelemetryOptions options);
}
