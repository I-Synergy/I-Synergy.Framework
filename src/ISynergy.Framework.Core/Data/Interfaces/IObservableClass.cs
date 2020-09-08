using System;
using System.Collections.ObjectModel;
using ISynergy.Framework.Core.Collections;

namespace ISynergy.Framework.Core.Data
{
    /// <summary>
    /// Interface IObservableClass
    /// Implements the <see cref="IBindable" />
    /// Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IBindable" />
    /// <seealso cref="IDisposable" />
    public interface IObservableClass : IBindable, IDisposable
    {
        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Validate();
        /// <summary>
        /// Reverts this instance.
        /// </summary>
        void Revert();
        /// <summary>
        /// Marks as clean.
        /// </summary>
        void MarkAsClean();
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        ObservableConcurrentDictionary<string, IProperty> Properties { get; }
        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        ObservableCollection<string> Errors { get; }
        /// <summary>
        /// Gets or sets the validator.
        /// </summary>
        /// <value>The validator.</value>
        Action<IObservableClass> Validator { set; get; }
        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        bool IsValid { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        bool IsDirty { get; }
    }
}
