using ISynergy.Framework.Core.Abstractions;

namespace ISynergy.Framework.Core.Models.Accounts;

/// <summary>
/// UserInfo model which fully supports serialization, property changed notifications,
/// backwards compatibility and error checking.
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
    /// Gets or sets the AccountId property value.
    /// </summary>
    /// <value>The account identifier.</value>
    public Guid AccountId { get; }

    /// <summary>
    /// Gets or sets the Account_Description property value.
    /// </summary>
    /// <value>The account description.</value>
    public string AccountDescription { get; }

    /// <summary>
    /// Gets or sets the TimeZoneId property value.
    /// </summary>
    /// <value>The time zone identifier.</value>
    public string TimeZoneId { get; }

    /// <summary>
    /// Gets the country code.
    /// </summary>
    public string CountryCode { get; }

    /// <summary>
    /// Gets or sets the UserId property value.
    /// </summary>
    /// <value>The user identifier.</value>
    public Guid UserId { get; }

    /// <summary>
    /// Gets or sets the Username property value.
    /// </summary>
    /// <value>The username.</value>
    public string Username { get; }

    /// <summary>
    /// Gets or sets the Email property value.
    /// </summary>
    /// <value>The email.</value>
    public string Email { get; }

    /// <summary>
    /// Gets or sets the Roles property value.
    /// </summary>
    /// <value>The roles.</value>
    public List<string> Roles { get; }

    /// <summary>
    /// Gets or sets the Modules property value.
    /// </summary>
    /// <value>The modules.</value>
    public List<string> Modules { get; }

    /// <summary>
    /// Gets or sets the License_Expration property value.
    /// </summary>
    /// <value>The license expration.</value>
    public DateTimeOffset LicenseExpration { get; }

    /// <summary>
    /// Gets or sets the License_Users property value.
    /// </summary>
    /// <value>The license users.</value>
    public int LicenseUsers { get; }

    /// <summary>
    /// Gets or sets the Token property value.
    /// </summary>
    /// <value>The token.</value>
    public Token Token { get; }

    /// <summary>
    /// Gets or sets the Expiration property value.
    /// </summary>
    /// <value>The expiration.</value>
    public DateTime Expiration { get; }

    /// <summary>
    /// Gets or sets the IsAuthenticated property value.
    /// </summary>
    /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
    public bool IsAuthenticated()
    {
        if (Token is not null && Expiration.CompareTo(DateTime.Now) >= 0 && LicenseExpration.CompareTo(DateTime.Now) >= 0)
            return true;
        return false;
    }

    /// <summary>
    /// Determines whether [is in role] [the specified role].
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns><c>true</c> if [is in role] [the specified role]; otherwise, <c>false</c>.</returns>
    public bool IsInRole(string role) => Roles?.Contains(role) ?? false;
}
