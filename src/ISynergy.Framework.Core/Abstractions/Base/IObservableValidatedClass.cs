using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Abstractions.Base;

/// <summary>
/// Interface IObservableClass
/// </summary>
public interface IObservableValidatedClass : IObservableClass, IAsyncDisposable, IDataErrorInfo, INotifyDataErrorInfo
{
    /// <summary>
    /// Validates this instance and optionally validates underlying properties.
    /// </summary>
    /// <param name="validateUnderlayingProperties">if set to <c>true</c>, validates underlying properties that implement <see cref="IObservableValidatedClass"/>.</param>
    /// <returns><c>true</c> if validation passed; otherwise, <c>false</c>.</returns>
    bool Validate(bool validateUnderlayingProperties = true);
    /// <summary>
    /// Gets the errors.
    /// </summary>
    /// <value>The errors.</value>
    ObservableCollection<KeyValuePair<string, string>> Errors { get; }
    /// <summary>
    /// Gets or sets the validator.
    /// </summary>
    /// <value>The validator.</value>
    Action<IObservableValidatedClass>? Validator { get; set; }
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
