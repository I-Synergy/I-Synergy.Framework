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

/// <summary>
/// Class UserSelect.
/// Implements the <see cref="User" />
/// </summary>
/// <seealso cref="User" />
public class UserSelect : User
{
    /// <summary>
    /// Gets or sets the IsSelected property value.
    /// </summary>
    /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
    public bool IsSelected { get; set; }
}

/// <summary>
/// Class UserAdd.
/// Implements the <see cref="User" />
/// </summary>
/// <seealso cref="User" />
public class UserAdd : User
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAdd"/> class.
    /// </summary>
    public UserAdd()
    {
        Id = Guid.NewGuid().ToString();
        Roles = new List<Role>();
    }

    /// <summary>
    /// Gets or sets the AccountId property value.
    /// </summary>
    /// <value>The account identifier.</value>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the Password property value.
    /// </summary>
    /// <value>The password.</value>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the Roles property value.
    /// </summary>
    /// <value>The roles.</value>
    public List<Role> Roles { get; set; }
}

/// <summary>
/// Class UserEdit.
/// Implements the <see cref="User" />
/// </summary>
/// <seealso cref="User" />
public class UserEdit : User
{
    public UserEdit()
        : base()
    {
        Roles = new List<Role>();
    }

    /// <summary>
    /// Gets or sets the IsConfirmed property value.
    /// </summary>
    /// <value><c>true</c> if this instance is confirmed; otherwise, <c>false</c>.</value>
    public bool IsConfirmed { get; set; }

    /// <summary>
    /// Gets or sets the Roles property value.
    /// </summary>
    /// <value>The roles.</value>
    public List<Role> Roles { get; set; }
}

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
