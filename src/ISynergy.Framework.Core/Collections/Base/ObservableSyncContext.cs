using System.ComponentModel;
using ISynergy.Framework.Core.Abstractions.Collections;
using ISynergy.Framework.Core.Helpers;

namespace ISynergy.Framework.Core.Collections.Base
{
    /// <summary>
    /// Contains all implementations for performing observable operations.
    /// </summary>
    public abstract class ObservableSyncContext : ISynchronizedObject
    {
        public ReadWriteLock ReadWriteLock { get; } = new ReadWriteLock(LockRecursionPolicy.SupportsRecursion);

        public SyncOperation SyncOperation { get; } = new SyncOperation();

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangedEventHandler? SynchronizedPropertyChanged;
        public event PropertyChangedEventHandler? UnsynchronizedPropertyChanged;

        /// <summary>
        /// Gets or sets <c>true</c> if the <see cref="PropertyChanged"/> event will invoke on the synchronized context.
        /// </summary>
        public bool SynchronizePropertyChangedEvent { get; set; }

        /// <summary>
        /// Raises <see cref="OnPropertyChanged(PropertyChangedEventArgs)"/> with the specified <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the changed property.
        /// </param>
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Invokes <see cref="PropertyChanged"/> and <see cref="SynchronizedPropertyChanged"/>.
        /// </summary>
        /// <param name="args">
        /// The <see cref="PropertyChangedEventArgs"/> event argument.
        /// </param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            UnsynchronizedPropertyChanged?.Invoke(this, args);
            SyncOperation.ContextPost(() =>
            {
                SynchronizedPropertyChanged?.Invoke(this, args);
            });

            if (SynchronizePropertyChangedEvent)
            {
                SyncOperation.ContextPost(() =>
                {
                    PropertyChanged?.Invoke(this, args);
                });
            }
            else
            {
                PropertyChanged?.Invoke(this, args);
            }
        }
    }
}
