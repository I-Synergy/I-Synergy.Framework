using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models;

namespace Sample.Models;

/// <summary>
/// Immutable user profile record.
/// </summary>
public sealed record class Profile(
     Token Token,
     Guid AccountId,
     string AccountDescription,
     string TimeZoneId,
     string CountryCode,
     Guid UserId,
     string Username,
     string Email,
     List<string> Roles,
     List<string> Modules,
     DateTimeOffset LicenseExpration,
     int LicenseUsers,
     DateTime Expiration) : IProfile
{
    /// <summary>
    /// Gets a value indicating whether this instance is authenticated.
    /// Determined by token/session expiration.
    /// </summary>
    public bool IsAuthenticated() =>
        DateTime.UtcNow < Expiration;

    /// <summary>
    /// Determines whether the user is in the specified role (case-insensitive).
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>true if the role is present; otherwise, false.</returns>
    public bool IsInRole(string role) =>
        !string.IsNullOrWhiteSpace(role) && Roles.Exists(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
}
