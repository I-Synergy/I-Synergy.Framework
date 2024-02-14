namespace ISynergy.Framework.Core.Attributes;

/// <summary>
/// Class SingletonAttribute. This class cannot be inherited.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class SingletonAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether this class is a singleton instance.
    /// </summary>
    /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
    public bool IsSingleton { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonAttribute" /> class.
    /// </summary>
    public SingletonAttribute(bool isSingleton)
    {
        IsSingleton = isSingleton;
    }
}
