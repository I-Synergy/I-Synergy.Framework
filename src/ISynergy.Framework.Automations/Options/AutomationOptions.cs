namespace ISynergy.Framework.Automations.Options
{
    /// <summary>
    /// Automation options.
    /// </summary>
    public class AutomationOptions
    {
        /// <summary>
        /// Default timeout.
        /// </summary>
        public TimeSpan DefaultTimeout { get; set; }
        /// <summary>
        /// Default refresh rate.
        /// </summary>
        public TimeSpan DefaultRefreshRate { get; set; }
        /// <summary>
        /// Default queue refresh rate.
        /// </summary>
        public TimeSpan DefaultQueueRefreshRate { get; set; }
    }
}
