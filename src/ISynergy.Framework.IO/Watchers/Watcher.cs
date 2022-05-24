using ISynergy.Framework.IO.Events;
using ISynergy.Framework.IO.Models;
using ISynergy.Framework.IO.Watchers.Base;
using System;
using System.IO;

namespace ISynergy.Framework.IO.Watchers
{
    /// <summary>
    /// Class Watcher.
    /// Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class Watcher : BaseWatcher<FileSystemWatcher, WatcherEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Watcher"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        public Watcher(WatcherInfo info)
            : base(info)
        {
        }
    }
}
