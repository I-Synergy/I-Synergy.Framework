using ISynergy.Framework.Core.Models;

namespace ISynergy.Framework.Core.Abstractions;

/// <summary>
/// Interface IProfile
/// </summary>
public interface IProfile
{
    Guid AccountId { get; set; }
    string Description { get; set; }
    string TimeZoneId { get; set; }
    string CountryCode { get; set; }
    string CultureCode { get; set; }
    Guid UserId { get; set; }
    string Username { get; set; }
    string Email { get; set; }
    List<string> Roles { get; set; }
    List<string> Modules { get; set; }
    DateTimeOffset Expiration { get; set; }
    Token Token { get; set; }
}
