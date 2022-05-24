using ISynergy.Framework.Telemetry.Abstractions.Options;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Telemetry.Options
{
    /// <summary>
    /// Telemetry options.
    /// </summary>
    public abstract class TelemetryOptions : ITelemetryOptions
    {
        /// <summary>
        /// Telemetry Api key.
        /// </summary>
        public string Key { get; set; }
    }
}
