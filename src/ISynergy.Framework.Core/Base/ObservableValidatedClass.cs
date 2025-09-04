using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Extensions;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ISynergy.Framework.Core.Base;

/// <summary>
/// Class ObservableClass.
/// Implements the <see cref="IObservableValidatedClass" />
/// </summary>
/// <seealso cref="IObservableValidatedClass" />
public abstract class ObservableValidatedClass : ObservableClass, IObservableValidatedClass
{
    protected EventHandler<DataErrorsChangedEventArgs>? _errorsChanged;

    /// <summary>
    /// Automatic validation trigger.
    /// </summary>
    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public bool AutomaticValidationTrigger
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the validator.
    /// </summary>
    /// <value>The validator.</value>
    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public Action<IObservableValidatedClass>? Validator { set; get; }

    /// <summary>
    /// Returns true if ... is valid.
    /// </summary>
    /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public bool IsValid => !HasErrors;

    /// <summary>
    /// Checks if both objects are the same based on common identity property.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is IObservableValidatedClass observable && observable.HasIdentityProperty())
            return observable.GetIdentityValue()!.Equals(this.GetIdentityValue());

        var currentPropertiesToCheck = this.GetType()
            .GetProperties()
            .Where(q =>
                !q.Name.Equals(nameof(IObservableValidatedClass.AutomaticValidationTrigger)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.Error)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.Errors)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.HasErrors)) &&
                !q.Name.Equals("Item") &&
                !q.Name.Equals(nameof(IObservableValidatedClass.IsDirty)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.IsValid)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.Properties)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.Validator)));

        if (obj is null)
            return false;

        var propertiesToCheck = obj.GetType()
            .GetProperties()
            .Where(q =>
                !q.Name.Equals(nameof(IObservableValidatedClass.AutomaticValidationTrigger)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.Error)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.Errors)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.HasErrors)) &&
                !q.Name.Equals("Item") &&
                !q.Name.Equals(nameof(IObservableValidatedClass.IsDirty)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.IsValid)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.Properties)) &&
                !q.Name.Equals(nameof(IObservableValidatedClass.Validator)));

        foreach (var property in currentPropertiesToCheck.EnsureNotNull())
        {
            if (!propertiesToCheck.Any(a => a.Name.Equals(property.Name)))
            {
                return false;
            }

            if (propertiesToCheck.SingleOrDefault(q => q.Name.Equals(property.Name)) is { } propertyInfo)
            {
                if ((property.GetValue(this) is null && propertyInfo.GetValue(obj) is not null) ||
                    (property.GetValue(this) is not null && propertyInfo.GetValue(obj) is null))
                {
                    return false;
                }

                if (property.GetValue(this) is not null && propertyInfo.GetValue(obj) is not null && !property.GetValue(this)!.Equals(propertyInfo.GetValue(obj)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        var identityValue = this.GetIdentityValue();

        if (identityValue is not null)
            return identityValue.GetHashCode();

        return base.GetHashCode();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidatedClass" /> class.
    /// </summary>
    /// <param name="automaticValidationTrigger">The validation.</param>
    protected ObservableValidatedClass(bool automaticValidationTrigger = false)
    {
        AutomaticValidationTrigger = automaticValidationTrigger;
        ErrorsChanged += ObservableClass_ErrorsChanged;
    }

    private void ObservableClass_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        RaisePropertyChanged(nameof(HasErrors));
        RaisePropertyChanged(nameof(Errors));
    }

    private void SetValueCore<T>(T? value, string? propertyName, bool shouldRaiseEvents)
    {
        if (propertyName is null)
            return;

        if (!Properties.ContainsKey(propertyName))
            Properties.Add(propertyName, new Property<T>(propertyName));

        if (Properties[propertyName] is not IProperty<T> property) return;

        // During cleanup or when events are suppressed, just set the value
        if (!shouldRaiseEvents || IsInCleanup)
        {
            property.Value = value;
            return;
        }

        ThrowIfDisposed();

        var previous = property.Value;
        bool shouldUpdate = !property.IsOriginalSet ||
                           !Equals(value, previous) ||
                           (typeof(T).IsNullableType() && value is null);

        if (shouldUpdate)
        {
            property.Value = value;
            RaisePropertyChanged(propertyName);

            if (AutomaticValidationTrigger)
                Validate();
        }
    }

    /// <summary>
    /// Clears the errors.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ClearErrors()
    {
        Errors.Clear();

        RaisePropertyChanged(nameof(IsValid));
        OnErrorsChanged(nameof(Errors));

        return IsValid;
    }

    /// <summary>
    /// Validates this instance.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Validate(bool validateUnderlayingProperties = true)
    {
        ClearErrors();

        if (validateUnderlayingProperties)
        {
            foreach (var property in this.GetType().GetProperties().Where(q => q.PropertyType.GetInterfaces().Contains(typeof(IObservableValidatedClass)) && !q.IsDefined(typeof(IgnoreValidationAttribute))).EnsureNotNull())
            {
                if (property.GetValue(this, null) is ObservableValidatedClass observable &&
                    !observable.Validate(validateUnderlayingProperties))
                {
                    Errors.AddRange(observable.Errors);
                    observable.Errors.Clear();
                }
            }
        }

        Validator?.Invoke(this);

        RaisePropertyChanged(nameof(IsValid));
        OnErrorsChanged(nameof(Errors));

        return IsValid;
    }

    /// <summary>
    /// Reverts this instance.
    /// </summary>
    public override void Revert()
    {
        base.Revert();

        if (AutomaticValidationTrigger)
            Validate();
    }

    /// <summary>
    /// Marks as clean.
    /// </summary>
    public override void MarkAsClean()
    {
        base.MarkAsClean();

        if (AutomaticValidationTrigger)
            Validate();
    }

    #region IDataErrorInfo
    public void AddValidationError(string propertyName, string errorMessage)
    {
        Errors.Add(new KeyValuePair<string, string>(propertyName, errorMessage));

        OnErrorsChanged(nameof(Errors));
        RaisePropertyChanged(nameof(IsValid));
    }


    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public ObservableCollection<KeyValuePair<string, string>> Errors { get; } = [];

    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public string Error
    {
        get
        {
            return string.Empty;
        }
    }

    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public string this[string propertyName]
    {
        get
        {
            if (!string.IsNullOrEmpty(propertyName) && Errors.Any(a => a.Key.Equals(propertyName)))
                return Errors
                    .FirstOrDefault(q => q.Key.Equals(propertyName))
                    .Value ?? propertyName;
            return string.Empty;
        }
    }
    #endregion

    #region INotifyDataErrorInfo Members

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged
    {
        add
        {
            ThrowIfDisposed();
            _errorsChanged += value;
        }
        remove
        {
            _errorsChanged -= value;
        }
    }

    protected virtual void OnErrorsChanged([CallerMemberName] string? propertyName = "")
    {
        ThrowIfDisposed();
        _errorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        if (!string.IsNullOrEmpty(propertyName))
        {
            if (Errors.Any(a => a.Key.Equals(propertyName)))
            {
                return Errors
                    .Where(q => q.Key.Equals(propertyName))
                    .Select(s => s.Value)
                    .ToList();
            }

            return Enumerable.Empty<string>();
        }

        return Errors.SelectMany(m => m.Value.ToList()).ToList();
    }

    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public bool HasErrors
    {
        get => Errors.Any(a => a.Value.Any());
    }

    public bool HasError(string property) => Errors.Any(a => a.Key.Equals(property));
    #endregion

    #region IDisposable & IAsyncDisposable
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            // free managed resources
            Validator = null;

            Errors?.Clear();

            if (_errorsChanged is not null)
            {
                foreach (var @delegate in _errorsChanged.GetInvocationList())
                {
                    if (@delegate is not null)
                        _errorsChanged -= (EventHandler<DataErrorsChangedEventArgs>)@delegate;
                }
            }
        }

        // free native resources if there are any.
        _disposed = true;
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await base.DisposeAsyncCore().ConfigureAwait(false);

        //Async dispose of managed resources
        Validator = null;

        Errors?.Clear();

        if (_errorsChanged is not null)
        {
            foreach (var @delegate in _errorsChanged.GetInvocationList())
            {
                if (@delegate is not null)
                    _errorsChanged -= (EventHandler<DataErrorsChangedEventArgs>)@delegate;
            }
        }

        _disposed = true;
    }
    #endregion
}
