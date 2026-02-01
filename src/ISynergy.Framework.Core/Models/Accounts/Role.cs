using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Models.Accounts;

/// <summary>
/// Class Role.
/// Implements the <see cref="BaseModel" />
/// </summary>
/// <seealso cref="BaseModel" />
public class Role : BaseModel
{
    /// <summary>
    /// Gets or sets the Id property value.
    /// </summary>
    /// <value>The identifier.</value>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the Name property value.
    /// </summary>
    /// <value>The name.</value>
    [Required]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    [JsonIgnore]
    public string? Description { get; set; }
}
