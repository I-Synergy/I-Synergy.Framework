using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Culture model which fully supports serialization, property changed notifications,
/// backwards compatibility and error checking.
/// </summary>
public class Culture : BaseModel
{
    /// <summary>
    /// Gets or sets the CultureId property value.
    /// </summary>
    /// <value>The culture identifier.</value>
    [Identity]
    [Required]
    public int CultureId { get; set; }

    /// <summary>
    /// Gets or sets the CountryId property value.
    /// </summary>
    /// <value>The country identifier.</value>
    [Required]
    public int CountryId { get; set; }

    /// <summary>
    /// Gets or sets the Language property value.
    /// </summary>
    /// <value>The language.</value>
    [Required]
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the TwoLetterLanguageCode property value.
    /// </summary>
    /// <value>The two letter language code.</value>
    [StringLength(5)]
    [Required]
    public string? TwoLetterLanguageCode { get; set; }

    /// <summary>
    /// Gets or sets the ThreeLetterLanguageCode property value.
    /// </summary>
    /// <value>The three letter language code.</value>
    [StringLength(5)]
    [Required]
    public string? ThreeLetterLanguageCode { get; set; }

    /// <summary>
    /// Gets or sets the CultureInfoCode property value.
    /// </summary>
    /// <value>The culture information code.</value>
    [Required]
    public string? CultureInfoCode { get; set; }
}
