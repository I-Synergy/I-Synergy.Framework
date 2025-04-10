using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Country model which fully supports serialization, property changed notifications,
/// backwards compatibility and error checking.
/// </summary>
public class Country : BaseModel
{
    /// <summary>
    /// Gets or sets the CountryId property value.
    /// </summary>
    /// <value>The country identifier.</value>
    [Identity]
    [Required]
    public int CountryId { get; set; }

    /// <summary>
    /// Gets or sets the ISO2Code property value.
    /// </summary>
    /// <value>The ISO2 code.</value>
    [Required]
    [StringLength(2)]
    public string? ISO2Code { get; set; }

    /// <summary>
    /// Gets or sets the CountryISO property value.
    /// </summary>
    /// <value>The country iso.</value>
    [Required]
    [StringLength(255)]
    public string? CountryISO { get; set; }

    /// <summary>
    /// Gets or sets the CountryPrefix property value.
    /// </summary>
    /// <value>The country prefix.</value>
    [Required]
    [StringLength(128)]
    public string? CountryPrefix { get; set; }

    /// <summary>
    /// Regex string for zipcode validation
    /// </summary>
    [Required]
    [StringLength(128)]
    public string? ZipCodeRegex { get; set; }

    /// <summary>
    /// Gets or sets the IsEU property value.
    /// </summary>
    /// <value><c>true</c> if this instance is eu; otherwise, <c>false</c>.</value>
    [Required]
    public bool IsEU { get; set; }

    /// <summary>
    /// Gets or sets the cultures.
    /// </summary>
    /// <value>The cultures.</value>
    public List<Culture> Cultures { get; set; }

    /// <summary>
    /// Gets or sets the Currency property value.
    /// </summary>
    /// <value>The currency.</value>
    public Currency? Currency { get; set; }

    public Country()
    {
        Cultures = new List<Culture>();
    }
}
