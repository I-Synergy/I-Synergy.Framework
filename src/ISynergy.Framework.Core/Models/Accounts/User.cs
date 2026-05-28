using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models.Accounts;

/// <summary>
/// Class User.
/// Implements the <see cref="BaseModel" />
/// </summary>
/// <seealso cref="BaseModel" />
public class User : BaseModel
{
    /// <summary>
    /// Gets or sets the Id property value.
    /// </summary>
    /// <value>The identifier.</value>
    [Required]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the UserName property value.
    /// </summary>
    /// <value>The name of the user.</value>
    [Required]
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the IsUnlocked property value.
    /// </summary>
    /// <value><c>true</c> if this instance is unlocked; otherwise, <c>false</c>.</value>
    [Required]
    public bool IsUnlocked { get; set; }
}
