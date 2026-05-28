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
    Task<string?> GetEnvironmentAsync();
    Task SaveTokensAsync(string? accessToken, string? refreshToken, string? idToken, DateTimeOffset? expiry, string environment);
    Task ClearTokensAsync();
}