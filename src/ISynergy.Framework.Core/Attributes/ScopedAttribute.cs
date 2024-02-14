namespace ISynergy.Framework.Core.Attributes;

/// <summary>
/// Class ScopedAttribute. This class cannot be inherited.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ScopedAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether this class is a scoped instance.
    /// </summary>
    /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
    public bool IsScoped { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedAttribute" /> class.
    /// </summary>
    public ScopedAttribute(bool isScoped)
    {
        IsScoped = isScoped;
    }
}