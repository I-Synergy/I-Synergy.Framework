using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;

namespace ISynergy.Framework.Core.Attributes;

/// <summary>
/// Class DescriptionAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class LocalizedDescriptionAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedDescriptionAttribute"/> class.
    /// </summary>
    /// <param name="resourceKey">The description.</param>
    public LocalizedDescriptionAttribute(string resourceKey)
    {
        Description = ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString(resourceKey);
    }
}
