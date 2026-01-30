namespace ISynergy.Framework.Core.Attributes;

/// <summary>
/// Class DescriptionAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class DescriptionAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionAttribute"/> class.
    /// </summary>
    /// <param name="description">The description.</param>
    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}
