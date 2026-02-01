namespace ISynergy.Framework.Core.Attributes;

/// <summary>
/// Class IdentityAttribute. This class cannot be inherited.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class IdentityAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is identity.
    /// </summary>
    /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
    public bool IsIdentity { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityAttribute"/> class.
    /// </summary>
    public IdentityAttribute()
    {
        IsIdentity = true;
    }
}
