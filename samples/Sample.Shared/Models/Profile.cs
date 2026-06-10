using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models;

namespace Sample.Models;

/// <summary>
/// Immutable user profile record.
/// </summary>
public sealed class Profile : IProfile
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the active tenant identifier.
    /// Equals UserId for personal accounts (no organizations),
    /// or the selected organization's ID when an organization is active.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the full name of the user or organization.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the time zone identifier.
    /// </summary>
    public required string TimeZoneId { get; set; }

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public required string CountryCode { get; set; }

    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public required string CultureCode { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the roles.
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Gets or sets the features.
    /// </summary>
    public List<string> Features { get; set; } = new();

    /// <summary>
    /// Gets or sets the license expiration.
    /// </summary>
    public DateTimeOffset Expiration { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets or sets the token.
    /// </summary>
    public required Token Token { get; set; }

    /// <summary>
    /// Gets or sets the organizations the user belongs to.
    /// </summary>
    public List<Organization> Organizations { get; set; } = new();

    IReadOnlyList<IOrganization> IProfile.Organizations => Organizations.Cast<IOrganization>().ToList();
}
