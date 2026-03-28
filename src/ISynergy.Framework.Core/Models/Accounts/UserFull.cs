using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models.Accounts;

/// <summary>
/// Class UserFull.
/// Implements the <see cref="User" />
/// </summary>
/// <seealso cref="User" />
public class UserFull : User
{
    public UserFull()
        : base()
    {
        Roles = new List<Role>();
    }

    /// <summary>
    /// Gets or sets the Roles property value.
    /// </summary>
    /// <value>The roles.</value>
    public List<Role> Roles { get; set; }

    /// <summary>
    /// Gets or sets the RolesSummary property value.
    /// </summary>
    /// <value>The roles summary.</value>
    public string RolesSummary
    {
        get
        {
            if (Roles is not null) return string.Join(", ", Roles.Select(s => s.Name));
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets or sets the IsConfirmed property value.
    /// </summary>
    /// <value><c>true</c> if this instance is confirmed; otherwise, <c>false</c>.</value>
    [Required]
    public bool IsConfirmed { get; set; }

    /// <summary>
    /// Gets or sets the FailedAttempts property value.
    /// </summary>
    /// <value>The failed attempts.</value>
    [Required]
    public int FailedAttempts { get; set; }
}
