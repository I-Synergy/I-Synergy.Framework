using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Attributes;

/// <summary>
/// Class LifetimeAttribute. This class cannot be inherited.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class LifetimeAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether this class is a scoped or singleton instance.
    /// </summary>
    public Lifetimes Lifetime { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LifetimeAttribute" /> class.
    /// </summary>
    public LifetimeAttribute(Lifetimes lifetime)
    {
        Lifetime = lifetime;
    }
}
