using ISynergy.Framework.Logging.Abstractions.Options;

namespace ISynergy.Framework.Logging.Options
{
    /// <summary>
    /// Logging options.
    /// </summary>
    public abstract class LoggerOptions : ILoggerOptions
    {
        /// <summary>
        /// Logging Api key.
        /// </summary>
        public string Key { get; set; }
    }
}
