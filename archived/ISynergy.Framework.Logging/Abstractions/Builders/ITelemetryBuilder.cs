using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Logging.Abstractions.Builders;

/// <summary>
/// Builder interface for configuring telemetry providers.
/// </summary>
public interface ITelemetryBuilder
{
    /// <summary>
    /// Gets the service collection to configure.
    /// </summary>
    IServiceCollection Services { get; }
}
