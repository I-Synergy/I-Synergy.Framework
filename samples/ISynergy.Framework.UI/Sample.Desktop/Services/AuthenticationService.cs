using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

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

        /// <summary>
        /// Occurs when authentication changed.
        /// </summary>
        public event EventHandler<ReturnEventArgs<bool>> AuthenticationChanged;

        /// <summary>
        /// Handles the <see cref="E:AuthenticationChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ReturnEventArgs{bool}"/> instance containing the event data.</param>
        public void OnAuthenticationChanged(ReturnEventArgs<bool> e) => AuthenticationChanged?.Invoke(this, e);

        public AuthenticationService(
            IContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _serviceScopeFactory = serviceScopeFactory;
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
            return Task.CompletedTask;
        }

        public Task AuthenticateWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            _authenticated = true;
            ValidateToken();
            return Task.CompletedTask;
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
            _authenticated = false;
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
