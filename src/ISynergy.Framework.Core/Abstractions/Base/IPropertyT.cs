namespace ISynergy.Framework.Core.Abstractions.Base;

/// <summary>
/// Interface IProperty
/// Implements the <see cref="IProperty" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="IProperty" />
public interface IProperty<T> : IProperty
{
    /// <summary>
    /// Gets or sets the original value.
    /// </summary>
    /// <value>The original value.</value>
    T? OriginalValue { get; set; }
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    T? Value { get; set; }
}
