using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Models.Accounts;

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
