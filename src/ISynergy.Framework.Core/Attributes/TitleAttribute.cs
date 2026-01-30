namespace ISynergy.Framework.Core.Attributes;

/// <summary>
/// Class TitleAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class TitleAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is a titel.
    /// </summary>
    /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
    public bool IsTitel { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TitleAttribute" /> class.
    /// </summary>
    public TitleAttribute()
    {
        IsTitel = true;
    }
}
