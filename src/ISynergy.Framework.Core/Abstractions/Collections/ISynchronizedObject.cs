using System.ComponentModel;
using ISynergy.Framework.Core.Collections.Base;
using ISynergy.Framework.Core.Helpers;

namespace ISynergy.Framework.Core.Abstractions.Collections
{
    /// <summary>
    /// Contains bundle declarations for observable operations.
    /// </summary>
    public interface ISynchronizedObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Event raised on the current synchronization context when a property is changed.
        /// </summary>
        event PropertyChangedEventHandler SynchronizedPropertyChanged;

        /// <summary>
        /// Event raised when a property is changed directly on the executing thread.
        /// </summary>
        event PropertyChangedEventHandler UnsynchronizedPropertyChanged;

        /// <summary>
        /// Gets the <see cref="SyncOperation"/> of the object.
        /// </summary>
        SyncOperation SyncOperation { get; }

        /// <summary>
        /// Gets the read-write lock for concurrency.
        /// </summary>
        ReadWriteLock ReadWriteLock { get; }
    }
}
