using ISynergy.Framework.Synchronization.Core.Abstractions;

namespace Sample.Synchronization.Common.Options
{
    /// <summary>
    /// Class that defines synchronization settings.
    /// </summary>
    public class FileSynchronizationOptions : IFileSynchronizationOptions
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
        /// Path of folder to be synchronized.
        /// </summary>
        public string SynchronizationFolderPath { get; set; }

        /// <summary>
        /// Url to the api route for getting the remote folder.
        /// </summary>
        /// <example>/api/sync/files/folders</example>
        public string SynchronizationFolderRoute { get; set; }

        /// <summary>
        /// Url to the api route for getting the remote list of files.
        /// </summary>
        /// <example>/api/sync/files/list</example>
        public string SynchronizationListRoute { get; set; }

        /// <summary>
        /// Url to the api route for getting the remote download.
        /// </summary>
        /// <example>/api/sync/files/download</example>
        public string SynchronizationDownloadRoute { get; set; }

        /// <summary>
        /// Parameter to the download api route.
        /// </summary>
        /// <example>path</example>
        public string SynchronizationDownloadParameter { get; set; }
    }
}
