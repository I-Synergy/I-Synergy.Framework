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
    /// <summary>
    /// Initializes a new instance of the <see cref="Account"/> class.
    /// </summary>
    public Account()
        : base()
    {
        AccountId = Guid.NewGuid();
        Modules = new List<Module>();
    }

    /// <summary>
    /// Gets or sets the Account_Id property value.
    /// </summary>
    /// <value>The account identifier.</value>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the CustomerId property value.
    /// </summary>
    /// <value>The relation identifier.</value>
    public Guid RelationId { get; set; }

    /// <summary>
    /// Gets or sets the Description property value.
    /// </summary>
    /// <value>The description.</value>
    [Required]
    [StringLength(128)]
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the Modules property value.
    /// </summary>
    /// <value>The modules.</value>
    [Required]
    public List<Module> Modules { get; set; }

    /// <summary>
    /// Gets or sets the UsersAllowed property value.
    /// </summary>
    /// <value>The users allowed.</value>
    [Required]
    public int UsersAllowed { get; set; }

    /// <summary>
    /// Gets or sets the RegistrationDate property value.
    /// </summary>
    /// <value>The registration date.</value>
    [Required]
    public DateTimeOffset RegistrationDate { get; set; }

    /// <summary>
    /// Gets or sets the ExpirationDate property value.
    /// </summary>
    /// <value>The expiration date.</value>
    [Required]
    public DateTimeOffset ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the IsActive property value.
    /// </summary>
    /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the TimeZoneId property value.
    /// </summary>
    /// <value>The time zone identifier.</value>
    [Required]
    public required string TimeZoneId { get; set; }

    /// <summary>
    /// Gets or sets the CountryCode property value.
    /// </summary>
    [Required]
    public required string CountryCode { get; set; }
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

    /// <summary>
    /// Gets or sets the Users property value.
    /// </summary>
    /// <value>The users.</value>
    public List<UserFull> Users { get; set; }
}
