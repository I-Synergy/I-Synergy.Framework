using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Sample.Services;

/// <summary>
/// Class AuthenticationService.
/// Implements the <see cref="IAuthenticationService" />
/// </summary>
/// <seealso cref="IAuthenticationService" />
public class AuthenticationService : IAuthenticationService
{
    private IContext _context;

    private readonly IScopedContextService _scopedContextService;
    private readonly ISettingsService _settingsService;
    private readonly ICredentialLockerService _credentialLockerService;
    private readonly ILogger _logger;

    /// <summary>
    /// Occurs when authentication changed.
    /// </summary>
    public event EventHandler<ReturnEventArgs<bool>> AuthenticationChanged;

    /// <summary>
    /// Handles the <see cref="E:AuthenticationChanged" /> event.
    /// </summary>
    public void OnAuthenticationChanged(ReturnEventArgs<bool> e) => AuthenticationChanged?.Invoke(this, e);

    public AuthenticationService(IScopedContextService scopedContextService, ILogger<AuthenticationService> logger)
    {
        _scopedContextService = scopedContextService;

        _context = scopedContextService.GetService<IContext>();
        _settingsService = scopedContextService.GetService<ISettingsService>();
        _credentialLockerService = scopedContextService.GetService<ICredentialLockerService>();

        _logger = logger;
        _logger.LogDebug($"AuthenticationService instance created with ID: {Guid.NewGuid()}");
    }

    public Task AuthenticateWithApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AuthenticateWithClientCredentialsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AuthenticateWithRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task AuthenticateWithUsernamePasswordAsync(string username, string password, bool remember, CancellationToken cancellationToken = default)
    {
        _context.Environment = SoftwareEnvironments.Test;
        _context.Profile = new Profile(
            new Token(),
            Guid.Parse("{79C13C79-B50B-4BEF-B796-294DED5676BB}"),
            "Test",
            "Europe/Amsterdam",
            "NL",
            Guid.NewGuid(),
            username,
            "admin@demo.com",
            ["Administrator"],
            [],
            DateTimeOffset.Now.AddDays(7),
            1,
            DateTime.Now.AddHours(24));

        if (remember)
        {
            if (!_settingsService.LocalSettings.IsAutoLogin ||
                _settingsService.LocalSettings.DefaultUser != username)
            {
                _settingsService.LocalSettings.IsAutoLogin = true;
                _settingsService.LocalSettings.DefaultUser = username;
                _settingsService.SaveLocalSettings();
            }

            await _credentialLockerService.AddCredentialToCredentialLockerAsync(username, password);
        }

        ValidateToken();
    }

    public Task<bool> CheckRegistrationEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Country>> GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public string GetEnvironmentalAuthToken(string token)
    {
        throw new NotImplementedException();
    }

    public Task<List<Module>> GetModulesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<Module>());
    }

    public Task<bool> RegisterNewAccountAsync(RegistrationData registration, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ResetPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SignOutAsync()
    {
        _scopedContextService.CreateNewScope();

        // Get the context from the new scope
        _context = _scopedContextService.GetService<IContext>();
        _context.Profile = null;

        ValidateToken();
        return Task.CompletedTask;
    }

    private void ValidateToken()
    {
        OnAuthenticationChanged(new ReturnEventArgs<bool>(_context.IsAuthenticated));
    }
}
