using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Sample.Abstractions;

namespace Sample.Services
{
    /// <summary>
    /// Class AuthenticationService.
    /// Implements the <see cref="IAuthenticationService" />
    /// </summary>
    /// <seealso cref="IAuthenticationService" />
    public class AuthenticationService : IAuthenticationService
    {
        private bool _authenticated;

        private readonly IContext _context;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBaseApplicationSettingsService _applicationSettingsService;
        private readonly ICredentialLockerService _credentialLockerService;

        /// <summary>
        /// Occurs when authentication changed.
        /// </summary>
        public event EventHandler<ReturnEventArgs<bool>> AuthenticationChanged;

        /// <summary>
        /// Handles the <see cref="E:AuthenticationChanged" /> event.
        /// </summary>
        /// <param name="e"></param>
        public void OnAuthenticationChanged(ReturnEventArgs<bool> e) => AuthenticationChanged?.Invoke(this, e);

        public AuthenticationService(
            IContext context,
            IServiceScopeFactory serviceScopeFactory,
            IBaseApplicationSettingsService applicationSettingsService,
            ICredentialLockerService credentialLockerService)
        {
            _context = context;
            _serviceScopeFactory = serviceScopeFactory;
            _applicationSettingsService = applicationSettingsService;
            _applicationSettingsService.LoadSettings();
            _credentialLockerService = credentialLockerService;
        }

        public Task AuthenticateWithApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AuthenticateWithClientCredentialsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AuthenticateWithRefreshTokenAsync(string refreshtoken, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task AuthenticateWithUsernamePasswordAsync(string username, string password, bool remember, CancellationToken cancellationToken = default)
        {
            _authenticated = true;

            if (remember)
            {
                if (_applicationSettingsService.Settings.IsAutoLogin != remember ||
                    _applicationSettingsService.Settings.DefaultUser != username)
                {
                    _applicationSettingsService.Settings.IsAutoLogin = true;
                    _applicationSettingsService.Settings.DefaultUser = username;
                    _applicationSettingsService.SaveSettings();
                }

                await _credentialLockerService.AddCredentialToCredentialLockerAsync(username, password);
            }

            ValidateToken();
        }

        public Task<bool> CheckRegistrationEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckRegistrationNameAsync(string name, CancellationToken cancellationToken = default)
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
            throw new NotImplementedException();
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
            _authenticated = false;

            if (!string.IsNullOrEmpty(_applicationSettingsService.Settings.DefaultUser))
            {
                _applicationSettingsService.Settings.IsAutoLogin = false;
                _applicationSettingsService.SaveSettings();
            }

            ValidateToken();
            return Task.CompletedTask;
        }

        private void ValidateToken()
        {
            _context.ScopedServices = _serviceScopeFactory.CreateScope();
            OnAuthenticationChanged(new ReturnEventArgs<bool>(_authenticated));
        }

    }
}
