using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.Core.Abstractions;

/// <summary>
/// Interface IProfile
/// </summary>
public interface IProfile
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the active tenant identifier.
    /// Equals <see cref="UserId"/> for personal accounts (no organizations),
    /// or the selected <see cref="IOrganization.OrganizationId"/> when an organization is active.
    /// </summary>
    Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the full name of the user or organization.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the time zone identifier.
    /// </summary>
    string TimeZoneId { get; set; }

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    string CountryCode { get; set; }

    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    string CultureCode { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    string Email { get; set; }

    /// <summary>
    /// Gets or sets the roles assigned to the user.
    /// </summary>
    List<string> Roles { get; set; }

    /// <summary>
    /// Gets or sets the features available to the user.
    /// </summary>
    List<string> Features { get; set; }

    /// <summary>
    /// Gets or sets the profile expiration.
    /// </summary>
    DateTimeOffset Expiration { get; set; }

    /// <summary>
    /// Gets or sets the authentication token.
    /// </summary>
    Token Token { get; set; }

    /// <summary>
    /// Gets the organizations the user belongs to.
    /// A user with zero organizations is a personal account.
    /// A user with one or more organizations can switch between them,
    /// which updates <see cref="TenantId"/> accordingly.
    /// </summary>
    IReadOnlyList<IOrganization> Organizations { get; }
}
