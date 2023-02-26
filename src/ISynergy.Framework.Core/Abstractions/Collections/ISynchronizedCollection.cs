using System.Collections.Specialized;

namespace ISynergy.Framework.Core.Abstractions.Collections
{
    /// <summary>
    /// Contains bundle declarations for observable operations.
    /// </summary>
    public interface ISynchronizedCollection : ISynchronizedObject, INotifyCollectionChanged
    {
        /// <summary>
        /// Event raised on the current synchronization context when the collection changes.
        /// </summary>
        event NotifyCollectionChangedEventHandler SynchronizedCollectionChanged;

        /// <summary>
        /// Event raised when a collection is changed directly on the executing thread.
        /// </summary>
        event NotifyCollectionChangedEventHandler UnsynchronizedCollectionChanged;
    }
}
