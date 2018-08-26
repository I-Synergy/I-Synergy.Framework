using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ISynergy.Factories
{
    /// <summary>
    /// Enables application insights to collect snapshots. This can happen for example when an exception occurs.
    /// </summary>
    /// <remarks>See: https://docs.microsoft.com/en-us/azure/application-insights/app-insights-snapshot-debugger/ </remarks>
    public class SnapshotCollectorTelemetryProcessorFactory : ITelemetryProcessorFactory
    {
        private readonly SnapshotCollectorConfiguration _configuration;

        public SnapshotCollectorTelemetryProcessorFactory(IOptions<SnapshotCollectorConfiguration> config) =>
            _configuration = config.Value;

        public ITelemetryProcessor Create(ITelemetryProcessor next)
        {
            return new SnapshotCollectorTelemetryProcessor(next, configuration: _configuration);
        }
    }
}
