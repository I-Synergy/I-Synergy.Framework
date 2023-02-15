using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Views;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;

namespace Sample.Services
{
    /// <summary>
    /// Class AuthenticationService.
    /// Implements the <see cref="IAuthenticationService" />
    /// </summary>
    /// <seealso cref="IAuthenticationService" />
    public class AuthenticationService : IAuthenticationService
    {
        private readonly INavigationService _navigationService;

        public AuthenticationService(INavigationService navigationService)
        {
            _navigationService = navigationService;
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
            Application.Current.MainPage = ServiceLocator.Default.GetInstance<IShellView>() as Page;
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

        public void SignOut() => _navigationService.ReplaceMainWindowAsync<IAuthenticationView>().Await();
    }
}
