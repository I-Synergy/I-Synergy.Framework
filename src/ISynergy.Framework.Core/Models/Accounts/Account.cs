using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models.Accounts;

/// <summary>
/// Class Account.
/// Implements the <see cref="BaseModel" />
/// </summary>
/// <seealso cref="BaseModel" />
public class Account : BaseModel
{
    public Account()
        : base()
    {
        AccountId = Guid.NewGuid();
        Modules = new List<Module>();
    }

    [Required]
    public Guid AccountId { get; set; }

    public Guid RelationId { get; set; }

    [Required]
    [StringLength(128)]
    public required string Description { get; set; }

    [Required]
    public List<Module> Modules { get; set; }

    [Required]
    public int UsersAllowed { get; set; }

    [Required]
    public DateTimeOffset RegistrationDate { get; set; }

    [Required]
    public DateTimeOffset ExpirationDate { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public required string TimeZoneId { get; set; }

    [Required]
    public required string CountryCode { get; set; }

    [Required]
    public required string CultureCode { get; set; }
}

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
