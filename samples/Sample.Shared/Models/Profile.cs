using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models;

namespace Sample.Models;

/// <summary>
/// Immutable user profile record.
/// </summary>
public sealed class Profile : IProfile
{
    /// <summary>
    /// Gets the account identifier.
    /// </summary>
    /// <value>The account identifier.</value>
    public Guid AccountId { get; set; }
    /// <summary>
    /// Gets the account description.
    /// </summary>
    /// <value>The account description.</value>
    public required string Description { get; set; }

    /// <summary>
    /// Gets the time zone identifier.
    /// </summary>
    /// <value>The time zone identifier.</value>
    public required string TimeZoneId { get; set; }

    /// <summary>
    /// Gets the country code.
    /// </summary>
    public required string CountryCode { get; set; }

    /// <summary>
    /// Gets the culture code.
    /// </summary>
    /// <value>The culture code.</value>
    public required string CultureCode { get; set; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets the username.
    /// </summary>
    /// <value>The username.</value>
    public required string Username { get; set; }

    /// <summary>
    /// Gets the email.
    /// </summary>
    /// <value>The email.</value>
    public required string Email { get; set; }

    /// <summary>
    /// Gets the roles.
    /// </summary>
    /// <value>The roles.</value>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Gets the modules.
    /// </summary>
    /// <value>The modules.</value>
    public List<string> Modules { get; set; } = new();

    /// <summary>
    /// Gets the license expration.
    /// </summary>
    /// <value>The license expration.</value>
    public DateTimeOffset Expiration { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    public required Token Token { get; set; }
}
