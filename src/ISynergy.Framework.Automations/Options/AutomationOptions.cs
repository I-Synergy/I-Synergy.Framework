namespace ISynergy.Framework.Automations.Options
{
    /// <summary>
    /// Automation options.
    /// </summary>
    public class AutomationOptions
    {
        /// <summary>
        /// Default automation timeout.
        /// </summary>
        public TimeSpan DefaultTimeout { get; set; }
        /// <summary>
        /// Default automation refresh rate.
        /// </summary>
        public TimeSpan DefaultRefreshRate { get; set; }
        /// <summary>
        /// Default automation refresh rate of the queue.
        /// </summary>
        public TimeSpan DefaultQueueRefreshRate { get; set; }
    }
}
