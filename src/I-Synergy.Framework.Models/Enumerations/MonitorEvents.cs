namespace ISynergy.Models.Enumerations
{
    /// <summary>
    /// Monitor event names.
    /// </summary>
    public enum MonitorEvents
    {
        /// <summary>
        /// When client is connected.
        /// </summary>
        Connected,
        /// <summary>
        /// When client is disconnected.
        /// </summary>
        Disconnected,
        /// <summary>
        /// Refresh message for a dashboard.
        /// </summary>
        RefreshDashboard,
        /// <summary>
        /// Notification of a new caller.
        /// </summary>
        NotifyCallerId
    }
}
