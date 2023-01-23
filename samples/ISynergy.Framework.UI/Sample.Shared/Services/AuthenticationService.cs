using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Services
{
    /// <summary>
    /// Class AuthenticationService.
    /// Implements the <see cref="IBaseAuthenticationService" />
    /// </summary>
    /// <seealso cref="IBaseAuthenticationService" />
    public class AuthenticationService : IAuthenticationService
    {
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
            throw new NotImplementedException();
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

        public void SignOut()
        {
            throw new NotImplementedException();
        }
    }
}
