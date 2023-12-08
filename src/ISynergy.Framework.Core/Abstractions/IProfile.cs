using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.Core.Abstractions;

/// <summary>
/// Interface IProfile
/// </summary>
public interface IProfile
{
    /// <summary>
    /// Gets the account identifier.
    /// </summary>
    /// <value>The account identifier.</value>
    Guid AccountId { get; }
    /// <summary>
    /// Gets the account description.
    /// </summary>
    /// <value>The account description.</value>
    string AccountDescription { get; }
    /// <summary>
    /// Gets the time zone identifier.
    /// </summary>
    /// <value>The time zone identifier.</value>
    string TimeZoneId { get; }
    /// <summary>
    /// Gets the country ISO2 code.
    /// </summary>
    string CountryCode { get; }
    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    Guid UserId { get; }
    /// <summary>
    /// Gets the username.
    /// </summary>
    /// <value>The username.</value>
    string Username { get; }
    /// <summary>
    /// Gets the email.
    /// </summary>
    /// <value>The email.</value>
    string Email { get; }
    /// <summary>
    /// Gets the roles.
    /// </summary>
    /// <value>The roles.</value>
    List<string> Roles { get; }
    /// <summary>
    /// Gets the modules.
    /// </summary>
    /// <value>The modules.</value>
    List<string> Modules { get; }
    /// <summary>
    /// Gets the license expration.
    /// </summary>
    /// <value>The license expration.</value>
    DateTimeOffset LicenseExpration { get; }
    /// <summary>
    /// Gets the license users.
    /// </summary>
    /// <value>The license users.</value>
    int LicenseUsers { get; }
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    Token Token { get; }
    /// <summary>
    /// Gets the expiration.
    /// </summary>
    /// <value>The expiration.</value>
    DateTime Expiration { get; }
    /// <summary>
    /// Gets a value indicating whether this instance is authenticated.
    /// </summary>
    /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
    bool IsAuthenticated();
    /// <summary>
    /// Determines whether [is in role] [the specified role].
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns><c>true</c> if [is in role] [the specified role]; otherwise, <c>false</c>.</returns>
    bool IsInRole(string role);
}
