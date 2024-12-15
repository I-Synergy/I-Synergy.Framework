using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Abstractions.Base;

/// <summary>
/// Interface IObservableClass
/// Implements the <see cref="IBindable" />
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IBindable" />
/// <seealso cref="IDisposable" />
public interface IObservableClass : IBindable, IDisposable, IAsyncDisposable, IDataErrorInfo, INotifyDataErrorInfo
{
    /// <summary>
    /// Gets or sets the IsDisposed property value.
    /// </summary>
    bool IsDisposed { get; }
    /// <summary>
    /// Validates this instance.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool Validate(bool validateUnderlayingProperties = true);
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
    Dictionary<string, IProperty> Properties { get; }
    /// <summary>
    /// Gets the errors.
    /// </summary>
    /// <value>The errors.</value>
    ObservableCollection<KeyValuePair<string, string>> Errors { get; }
    /// <summary>
    /// Gets or sets the validator.
    /// </summary>
    /// <value>The validator.</value>
    Action<IObservableClass> Validator { get; set; }
    /// <summary>
    /// Gets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
    bool IsDirty { get; }
    /// <summary>
    /// Returns true if ... is valid.
    /// </summary>
    /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
    bool IsValid { get; }
    /// <summary>
    /// Automatic validation trigger.
    /// </summary>
    bool AutomaticValidationTrigger { get; set; }
    /// <summary>
    /// Adds an error message to the validation errors.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="errorMessage"></param>
    void AddValidationError(string propertyName, string errorMessage);
}
