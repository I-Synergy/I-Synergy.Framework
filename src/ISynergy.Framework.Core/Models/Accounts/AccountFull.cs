using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Models.Accounts;

/// <summary>
/// Class AccountFull.
/// Implements the <see cref="Account" />
/// </summary>
/// <seealso cref="Account" />
public class AccountFull : Account
{
    public AccountFull()
        : base()
    {
        Users = new List<UserFull>();
    }

    public List<UserFull> Users { get; set; }
}
