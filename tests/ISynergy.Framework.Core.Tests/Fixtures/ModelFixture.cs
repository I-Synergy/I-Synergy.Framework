using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Fixtures;

/// <summary>
/// Class ModelFixture.
/// Implements the <see cref="BaseModel" />
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="BaseModel" />
/// <seealso cref="IDisposable" />
public class ModelFixture<T> : BaseModel, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelFixture{T}"/> class.
    /// </summary>
    public ModelFixture()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelFixture{T}"/> class.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    public ModelFixture(T initialValue)
        : this()
    {
        Value = initialValue;
    }

    /// <summary>
    /// Gets or sets the Value property value.
    /// </summary>
    /// <value>The value.</value>
    public T Value
    {
        get { return GetValue<T>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString()
    {
        return Value is null ? string.Empty : Value.ToString()!;
    }
}
