using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Models.Accounts;

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
