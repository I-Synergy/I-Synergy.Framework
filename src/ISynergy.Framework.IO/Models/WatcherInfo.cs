using System.IO;

namespace ISynergy.Framework.IO.Models
{
    /// <summary>
    /// Class WatcherInfo.
    /// </summary>
    public class WatcherInfo
    {
        /// <summary>
        /// Gets or sets the watch path.
        /// </summary>
        /// <value>The watch path.</value>
        public string WatchPath { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [include sub folders].
        /// </summary>
        /// <value><c>true</c> if [include sub folders]; otherwise, <c>false</c>.</value>
        public bool IncludeSubFolders { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [watch for error].
        /// </summary>
        /// <value><c>true</c> if [watch for error]; otherwise, <c>false</c>.</value>
        public bool WatchForError { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [watch for disposed].
        /// </summary>
        /// <value><c>true</c> if [watch for disposed]; otherwise, <c>false</c>.</value>
        public bool WatchForDisposed { get; set; }
        /// <summary>
        /// Gets or sets the changes filters.
        /// </summary>
        /// <value>The changes filters.</value>
        public NotifyFilters ChangesFilters { get; set; }
        /// <summary>
        /// Gets or sets the watches filters.
        /// </summary>
        /// <value>The watches filters.</value>
        public WatcherChangeTypes WatchesFilters { get; set; }
        /// <summary>
        /// Gets or sets the file filter.
        /// </summary>
        /// <value>The file filter.</value>
        public string FileFilter { get; set; }
        /// <summary>
        /// Gets or sets the buffer k bytes.
        /// </summary>
        /// <value>The buffer k bytes.</value>
        public uint BufferKBytes { get; set; }

        // only applicable if using WatcherEx class
        /// <summary>
        /// Gets or sets the monitor path interval.
        /// </summary>
        /// <value>The monitor path interval.</value>
        public int MonitorPathInterval { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherInfo"/> class.
        /// </summary>
        public WatcherInfo()
        {
            WatchPath = "";
            IncludeSubFolders = false;
            WatchForError = false;
            WatchForDisposed = false;
            ChangesFilters = NotifyFilters.Attributes;
            WatchesFilters = WatcherChangeTypes.All;
            FileFilter = "";
            BufferKBytes = 8;
            MonitorPathInterval = 0;
        }
    }
}
