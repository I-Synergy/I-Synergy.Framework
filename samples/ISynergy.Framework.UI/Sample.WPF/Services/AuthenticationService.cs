using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Services;
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
        private readonly IContext _context;
        private readonly INavigationService _navigationService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AuthenticationService(
            IContext context,
            INavigationService navigationService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _navigationService = navigationService;
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
            throw new NotImplementedException();
        }

        public Task AuthenticateWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
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

        public async Task SignOutAsync()
        {
            ValidateToken();
            await _navigationService.CleanBackStackAsync();
        }

        private void ValidateToken()
        {
            _context.ScopedServices = _serviceScopeFactory.CreateScope();
        }
    }
}
