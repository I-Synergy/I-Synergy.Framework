using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ISynergy.Framework.Core.Base;

public abstract class ObservableClass : IObservableClass
{
    protected PropertyChangedEventHandler? _propertyChanged;
    protected bool _disposed;

    /// <summary>
    /// Gets or sets the IsDisposed property value.
    /// </summary>
    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public bool IsDisposed
    {
        get => _disposed;
    }

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <value>The properties.</value>
    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public Dictionary<string, IProperty> Properties { get; } = new Dictionary<string, IProperty>();

    /// <summary>
    /// Gets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    public bool IsDirty
    {
        get { return Properties.Any(x => x.Value.IsDirty); }
    }

    /// <summary>
    /// Checks if both objects are the same based on common identity property.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        var currentPropertiesToCheck = this.GetType()
            .GetProperties()
            .Where(q =>
                !q.Name.Equals("Item") &&
                !q.Name.Equals(nameof(IObservableClass.IsDirty)) &&
                !q.Name.Equals(nameof(IObservableClass.Properties)));

        if (obj is null)
            return false;

        var propertiesToCheck = obj.GetType()
            .GetProperties()
            .Where(q =>
                !q.Name.Equals("Item") &&
                !q.Name.Equals(nameof(IObservableClass.IsDirty)) &&
                !q.Name.Equals(nameof(IObservableClass.Properties)));

        foreach (var property in currentPropertiesToCheck.EnsureNotNull())
        {
            if (!propertiesToCheck.Any(a => a.Name.Equals(property.Name)))
                return false;

            if (propertiesToCheck.SingleOrDefault(q => q.Name.Equals(property.Name)) is { } propertyInfo)
            {
                if ((property.GetValue(this) is null && propertyInfo.GetValue(obj) is not null) ||
                    (property.GetValue(this) is not null && propertyInfo.GetValue(obj) is null))
                {
                    return false;
                }

                if (property.GetValue(this) is not null && propertyInfo.GetValue(obj) is not null && !property.GetValue(this)!.Equals(propertyInfo.GetValue(obj)))
                    return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() =>
        new HashCode().ToHashCode();

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>T.</returns>
    protected T GetValue<T>([CallerMemberName] string? propertyName = "")
    {
        if (propertyName is not null && !Properties.ContainsKey(propertyName))
            Properties.Add(propertyName, new Property<T>(propertyName));

        if (propertyName is not null && Properties[propertyName] is IProperty<T> property)
            return property.Value!;

        return default!;
    }

    [JsonIgnore]
    [DataTableIgnore]
    [XmlIgnore]
    [Display(AutoGenerateField = false)]
    protected bool IsInCleanup { get; set; }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="propertyName">Name of the property.</param>
    protected void SetValue<T>(T value, [CallerMemberName] string? propertyName = "")
    {
        SetValueCore<T>(value, propertyName, !IsInCleanup);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <param name="propertyName">Name of the property.</param>
    protected void SetValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = "")
    {
        SetValueCore<T>(value, propertyName, !IsInCleanup);
        field = value;
    }

    private void SetValueCore<T>(T value, string? propertyName, bool shouldRaiseEvents)
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
        }
    }

    /// <summary>
    /// Reverts this instance.
    /// </summary>
    public virtual void Revert()
    {
        foreach (var property in Properties.EnsureNotNull())
            property.Value.ResetChanges();
    }

    /// <summary>
    /// Marks as clean.
    /// </summary>
    public virtual void MarkAsClean()
    {
        foreach (var property in Properties.EnsureNotNull())
            property.Value.MarkAsClean();
    }

    #region INotifyPropertyChanged
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <returns></returns>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            ThrowIfDisposed();
            _propertyChanged += value;
        }
        remove
        {
            _propertyChanged -= value;
        }
    }

    /// <summary>
    /// Called when [property changed].
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = "")
    {
        ThrowIfDisposed();
        _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region IDisposable & IAsyncDisposable
    // Dispose() calls Dispose(true)
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // free managed resources
            if (Properties is not null)
            {
                //Dispose of all properties that implement IDisposable
                foreach (var property in Properties.Values)
                {
                    (property as IDisposable)?.Dispose();
                }

                Properties.Clear();
            }

            // Clear event handlers to prevent memory leaks
            // Since _propertyChanged is a field-backed event, we can simply set it to null
            _propertyChanged = null;
        }

        // free native resources if there are any.
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;

        //Async dispose of managed resources
        if (Properties is not null)
        {
            //Dispose of all properties that implement IAsyncDisposable
            foreach (var property in Properties.Values)
            {
                if (property is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }

            Properties.Clear();
        }

        // Clear event handlers to prevent memory leaks
        // Since _propertyChanged is a field-backed event, we can simply set it to null
        _propertyChanged = null;

        _disposed = true;
    }

    protected void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }
    #endregion
}
