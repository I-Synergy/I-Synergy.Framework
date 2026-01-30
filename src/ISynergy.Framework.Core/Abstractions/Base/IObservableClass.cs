namespace ISynergy.Framework.Core.Abstractions.Base;
public interface IObservableClass : IBindable, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets or sets the IsDisposed property value.
    /// </summary>
    bool IsDisposed { get; }
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
    /// Gets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
    bool IsDirty { get; }
}