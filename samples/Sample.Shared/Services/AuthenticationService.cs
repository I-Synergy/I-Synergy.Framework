using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
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
        var context = _scopedContextService.GetService<IContext>();
        context.Environment = SoftwareEnvironments.Test;

        context.Profile = new Profile(
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
            var settingsService = _scopedContextService.GetService<ISettingsService>();

            if (settingsService.LocalSettings.DefaultUser != username)
            {
                settingsService.LocalSettings.IsAutoLogin = true;
                settingsService.LocalSettings.DefaultUser = username;
                settingsService.SaveLocalSettings();
            }

            var credentialLockerService = _scopedContextService.GetService<ICredentialLockerService>();
            await credentialLockerService.AddCredentialToCredentialLockerAsync(username, password);
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
        ValidateToken(null);
    }

    private void ValidateToken(Token token)
    {
        if (token is not null)
        {
            var context = _scopedContextService.GetService<IContext>();
            context.Profile = new Profile(
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

            OnAuthenticationChanged(new ReturnEventArgs<bool>(context.IsAuthenticated));

            MessageService.Default.Send(new EnvironmentChangedMessage(context.Environment));
        }
        else
        {
            OnAuthenticationChanged(new ReturnEventArgs<bool>(false));
        }

    }
}
