using Microsoft.Extensions.Logging;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Abstractions.Services;

#if WINDOWS
using Windows.Security.Credentials;
#elif ANDROID
using Android.Security.Keystore;
#elif IOS || MACCATALYST
using Foundation;
using Security;
#endif

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Platform-specific secure token storage implementation
/// </summary>
public class TokenStorageService : ITokenStorageService
{
    private readonly IInfoService _infoService;
    private readonly ILogger<TokenStorageService> _logger;

    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string IdTokenKey = "id_token";
    private const string TokenExpiryKey = "token_expiry";
    private const string EnvironmentKey = "environment";

    public TokenStorageService(IInfoService infoService, ILogger<TokenStorageService> logger)
    {
        _infoService = infoService;
        _logger = logger;
    }

    public Task<string?> GetAccessTokenAsync() =>
        GetSecureValueAsync(AccessTokenKey);

    public Task<string?> GetRefreshTokenAsync() =>
        GetSecureValueAsync(RefreshTokenKey);

    public Task<string?> GetIdTokenAsync() =>
        GetSecureValueAsync(IdTokenKey);

    public async Task<DateTimeOffset?> GetTokenExpiryAsync()
    {
        var expiryString = await GetSecureValueAsync(TokenExpiryKey);
        if (string.IsNullOrEmpty(expiryString))
            return null;

        if (DateTimeOffset.TryParse(expiryString, out var expiry))
            return expiry;

        return null;
    }

    public async Task<SoftwareEnvironments?> GetEnvironmentAsync()
    {
        var environmentString = await GetSecureValueAsync(EnvironmentKey);
        if (string.IsNullOrEmpty(environmentString))
            return null;

        if (Enum.TryParse<SoftwareEnvironments>(environmentString, out var environment))
            return environment;

        return null;
    }

    public async Task SaveTokensAsync(string? accessToken, string? refreshToken, string? idToken, DateTimeOffset? expiry, SoftwareEnvironments? environment)
    {
        try
        {
            if (!string.IsNullOrEmpty(accessToken))
                await SetSecureValueAsync(AccessTokenKey, accessToken);

            if (!string.IsNullOrEmpty(refreshToken))
                await SetSecureValueAsync(RefreshTokenKey, refreshToken);

            if (!string.IsNullOrEmpty(idToken))
                await SetSecureValueAsync(IdTokenKey, idToken);

            if (expiry.HasValue)
                await SetSecureValueAsync(TokenExpiryKey, expiry.Value.ToString("O"));

            if (environment.HasValue)
                await SetSecureValueAsync(EnvironmentKey, environment.Value.ToString());


            _logger.LogDebug("Tokens saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save tokens");
            throw;
        }
    }

    public async Task ClearTokensAsync()
    {
        try
        {
            await DeleteSecureValueAsync(AccessTokenKey);
            await DeleteSecureValueAsync(RefreshTokenKey);
            await DeleteSecureValueAsync(IdTokenKey);
            await DeleteSecureValueAsync(TokenExpiryKey);
            await DeleteSecureValueAsync(EnvironmentKey);

            _logger.LogDebug("Tokens cleared successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear tokens");
        }
    }

    private Task<string?> GetSecureValueAsync(string key)
    {
#if WINDOWS
        try
        {
            var vault = new PasswordVault();
            var credential = vault.Retrieve(_infoService.ProductName, key);
            credential.RetrievePassword();
            return Task.FromResult<string?>(credential.Password);
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
#elif ANDROID
        var preferences = Android.App.Application.Context.GetSharedPreferences(_infoService.ProductName, Android.Content.FileCreationMode.Private);
        return Task.FromResult<string?>(preferences?.GetString(key, null));
#elif IOS || MACCATALYST
        var record = new SecRecord(SecKind.GenericPassword)
        {
            Service = _infoService.ProductName,
            Account = key
        };
        
        // FIX: Use NSString instead of ToString()
        var data = SecKeyChain.QueryAsData(record, true, out var resultCode);
        if (resultCode == SecStatusCode.Success && data != null)
        {
            return Task.FromResult<string?>(NSString.FromData(data, NSStringEncoding.UTF8));
        }
        return Task.FromResult<string?>(null);
#else
        return Task.FromResult<string?>(null);
#endif
    }

    private Task SetSecureValueAsync(string key, string value)
    {
#if WINDOWS
        try
        {
            var vault = new PasswordVault();
            try
            {
                var oldCredential = vault.Retrieve(_infoService.ProductName, key);
                vault.Remove(oldCredential);
            }
            catch { }

            vault.Add(new PasswordCredential(_infoService.ProductName, key, value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store value in Windows PasswordVault");
            throw;
        }
#elif ANDROID
        var preferences = Android.App.Application.Context.GetSharedPreferences(_infoService.ProductName, Android.Content.FileCreationMode.Private);
        var editor = preferences?.Edit();
        editor?.PutString(key, value);
        editor?.Apply();
#elif IOS || MACCATALYST
        var record = new SecRecord(SecKind.GenericPassword)
        {
            Service = _infoService.ProductName,
            Account = key,
            ValueData = NSData.FromString(value)
        };
        
        SecKeyChain.Remove(record);
        SecKeyChain.Add(record);
#endif
        return Task.CompletedTask;
    }

    private Task DeleteSecureValueAsync(string key)
    {
#if WINDOWS
        try
        {
            var vault = new PasswordVault();
            var credential = vault.Retrieve(_infoService.ProductName, key);
            vault.Remove(credential);
        }
        catch { }
#elif ANDROID
        var preferences = Android.App.Application.Context.GetSharedPreferences(_infoService.ProductName, Android.Content.FileCreationMode.Private);
        var editor = preferences?.Edit();
        editor?.Remove(key);
        editor?.Apply();
#elif IOS || MACCATALYST
        var record = new SecRecord(SecKind.GenericPassword)
        {
            Service = _infoService.ProductName,
            Account = key
        };
        SecKeyChain.Remove(record);
#endif
        return Task.CompletedTask;
    }
}