using ISynergy.Framework.Telemetry.Abstractions.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ISynergy.Framework.Telemetry.AppCenter")]
[assembly: InternalsVisibleTo("ISynergy.Framework.Telemetry.ApplicationInsights")]
namespace ISynergy.Framework.Telemetry.Options
{
    /// <summary>
    /// Telemetry options.
    /// </summary>
    internal abstract class TelemetryOptions : ITelemetryOptions
    {
        /// <summary>
        /// Telemetry Api key.
        /// </summary>
        public string Key { get; set; }
    }
}
