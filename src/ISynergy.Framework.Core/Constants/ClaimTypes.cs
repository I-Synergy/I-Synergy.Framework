namespace ISynergy.Framework.Core.Constants;

/// <summary>
/// Class ClaimTypes.
/// </summary>
public static class ClaimTypes
{
    //Used by identity token
    /// <summary>
    /// The account identifier type
    /// </summary>
    public const string AccountIdType = "account_id";
    /// <summary>
    /// The account description type
    /// </summary>
    public const string AccountDescriptionType = "account_description";
    /// <summary>
    /// The user name type
    /// </summary>
    public const string UserNameType = "username";
    /// <summary>
    /// The user identifier type
    /// </summary>
    public const string UserIdType = "user_id";
    /// <summary>
    /// The client identifier type
    /// </summary>
    public const string ClientIdType = "client_id";
    /// <summary>
    /// The rfid uid type
    /// </summary>
    public const string RfidUidType = "rfid_uid";
    /// <summary>
    /// The role type
    /// </summary>
    public const string RoleType = "user_role";
    /// <summary>
    /// The modules type
    /// </summary>
    public const string ModulesType = "modules";
    /// <summary>
    /// The license expration type
    /// </summary>
    public const string LicenseExprationType = "license_expration";
    /// <summary>
    /// The license users type
    /// </summary>
    public const string LicenseUsersType = "license_users";
    /// <summary>
    /// The security stamp type
    /// </summary>
    public const string SecurityStampType = "security_stamp";
    /// <summary>
    /// The time zone type
    /// </summary>
    public const string TimeZoneType = "timezone";
    /// <summary>
    /// The country claim type.
    /// </summary>
    public const string CountryType = "country";
    /// <summary>
    /// The expiration type
    /// </summary>
    public const string ExpirationType = "exp";

    //Used by role-based claims
    /// <summary>
    /// The permission type
    /// </summary>
    public const string PermissionType = "permission";

    /// <summary>
    /// Gets the authorization code claim types.
    /// </summary>
    /// <value>The authorization code claim types.</value>
    public static IEnumerable<string> AuthorizationCodeClaimTypes =>
        new[] { UserIdType, UserNameType, SecurityStampType };

    /// <summary>
    /// Gets the access token claim types.
    /// </summary>
    /// <value>The access token claim types.</value>
    public static IEnumerable<string> AccessTokenClaimTypes
        => new[] { AccountIdType, AccountDescriptionType, UserNameType, UserIdType, RfidUidType, RoleType, ModulesType, LicenseExprationType, LicenseUsersType };

    /// <summary>
    /// Gets the refresh token claim types.
    /// </summary>
    /// <value>The refresh token claim types.</value>
    public static IEnumerable<string> RefreshTokenClaimTypes
        => new[] { UserNameType, UserIdType, RfidUidType, SecurityStampType };

    /// <summary>
    /// Gets the introspect claim types.
    /// </summary>
    /// <value>The introspect claim types.</value>
    public static IEnumerable<string> IntrospectClaimTypes =>
        new[] { AccountIdType };
}
