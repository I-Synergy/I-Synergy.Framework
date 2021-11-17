using ISynergy.Framework.Telemetry.Abstractions.Options;

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
