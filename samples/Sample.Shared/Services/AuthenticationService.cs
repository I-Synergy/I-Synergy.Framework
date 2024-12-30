using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Core.Services;
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
    private ISettingsService _settingsService;
    private ICredentialLockerService _credentialLockerService;

    private readonly IScopedContextService _scopedContextService;
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

        _logger = logger;
        _logger.LogDebug($"AuthenticationService instance created with ID: {Guid.NewGuid()}");

        SignOut();
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

        ValidateToken(new Token());
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
        return $"{GenericConstants.UsernamePrefixTest}{token}";
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

    public void SignOut()
    {
        _scopedContextService.CreateNewScope();

        _settingsService = _scopedContextService.GetService<ISettingsService>();
        _credentialLockerService = _scopedContextService.GetService<ICredentialLockerService>();

        _context = _scopedContextService.GetService<IContext>();
        _context.Profile = null;

        ValidateToken(null);
    }

    private void ValidateToken(Token token)
    {
        if (token is not null)
        {
            _context.Profile = new Profile(
                token,
                Guid.Parse("{79C13C79-B50B-4BEF-B796-294DED5676BB}"),
                "Test",
                "Europe/Amsterdam",
                "NL",
                Guid.NewGuid(),
                "admin",
                "admin@demo.com",
                ["Administrator"],
                [],
                DateTimeOffset.Now.AddDays(7),
                1,
                DateTime.Now.AddHours(24));

            _settingsService.LocalSettings.RefreshToken = GetEnvironmentalAuthToken(token?.RefreshToken);
            _settingsService.SaveLocalSettings();

            OnAuthenticationChanged(new ReturnEventArgs<bool>(_context.IsAuthenticated));

            MessageService.Default.Send(new EnvironmentChangedMessage(_context.Environment));
        }
        else
        {
            OnAuthenticationChanged(new ReturnEventArgs<bool>(false));
        }

    }
}
