namespace Sample.Synchronization.Common.Options
{
    /// <summary>
    /// Class that defines synchronization settings.
    /// </summary>
    public class ServerSynchronizationOptions
    {
        /// <summary>
        /// Host name or ip address of the service endpoint.
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Interval in which the client checks if host is available.
        /// </summary>
        public TimeSpan CheckHostInterval { get; set; }
        /// <summary>
        /// Channel (Group name) for synchronization clients.
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// List of all tables to be synchronized.
        /// </summary>
        public string[] Tables { get; set; }
        /// <summary>
        /// List of all folders to be synchronized.
        /// </summary>
        public string[] Folders { get; set; }
    }
}
