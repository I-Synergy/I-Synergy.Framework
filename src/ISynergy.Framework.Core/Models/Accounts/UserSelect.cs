using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Models.Accounts;

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
