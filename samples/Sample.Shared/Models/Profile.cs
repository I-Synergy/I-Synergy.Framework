using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models;

namespace Sample.Models;

/// <summary>
/// Class Profile.
/// </summary>
public class Profile : IProfile
{
    public Profile(
        Token token,
        Guid accountId,
        string accountDescription,
        string timeZoneId,
        string countryCode,
        Guid userId,
        string username,
        string email,
        List<string> roles,
        List<string> modules,
        DateTimeOffset licenseExpration,
        int licenseUsers,
        DateTime expiration)
    {
        AccountId = accountId;
        AccountDescription = accountDescription;
        TimeZoneId = timeZoneId;
        CountryCode = countryCode;
        UserId = userId;
        Username = username;
        Email = email;
        Roles = roles;
        Modules = modules;
        LicenseExpration = licenseExpration;
        LicenseUsers = licenseUsers;
        Token = token;
        Expiration = expiration;
    }

    /// <summary>
    /// Gets the account identifier.
    /// </summary>
    /// <value>The account identifier.</value>
    public Guid AccountId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets the account description.
    /// </summary>
    /// <value>The account description.</value>
    public string AccountDescription { get; set; } = "Sample";

    /// <summary>
    /// Gets the time zone identifier.
    /// </summary>
    /// <value>The time zone identifier.</value>
    public string TimeZoneId { get; set; } = "W. Europe Standard Time";

    /// <summary>
    /// Gets the country code.
    /// </summary>
    public string CountryCode { get; set; } = "nl";

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    public Guid UserId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets the username.
    /// </summary>
    /// <value>The username.</value>
    public string Username { get; set; } = "Anonymous";

    /// <summary>
    /// Gets the email.
    /// </summary>
    /// <value>The email.</value>
    public string Email { get; set; } = "user@demo.com";

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
    /// <exception cref="NotImplementedException"></exception>
    public DateTimeOffset LicenseExpration { get; set; }

    /// <summary>
    /// Gets the license users.
    /// </summary>
    /// <value>The license users.</value>
    public int LicenseUsers { get; set; } = 1;

    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>The token.</value>
    /// <exception cref="NotImplementedException"></exception>
    public Token Token { get; set; }

    /// <summary>
    /// Gets the expiration.
    /// </summary>
    /// <value>The expiration.</value>
    /// <exception cref="NotImplementedException"></exception>
    public DateTime Expiration { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is authenticated.
    /// </summary>
    /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
    public bool IsAuthenticated() => true;

    /// <summary>
    /// Determines whether [is in role] [the specified role].
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns><c>true</c> if [is in role] [the specified role]; otherwise, <c>false</c>.</returns>
    public bool IsInRole(string role) => true;
}
