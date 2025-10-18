using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.UI.Abstractions.Services;

/// <summary>
/// Service for securely storing and retrieving authentication tokens
/// </summary>
public interface ITokenStorageService
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task<string?> GetIdTokenAsync();
    Task<DateTimeOffset?> GetTokenExpiryAsync();
    Task<SoftwareEnvironments?> GetEnvironmentAsync();
    Task SaveTokensAsync(string? accessToken, string? refreshToken, string? idToken, DateTimeOffset? expiry, SoftwareEnvironments? environment);
    Task ClearTokensAsync();
}