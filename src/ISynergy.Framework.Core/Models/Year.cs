namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Class Year.
/// </summary>
public record Year
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public int Value { get; set; }
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; } = string.Empty;
}
