using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Sample.Services
{
    /// <summary>
    /// Class AuthenticationService.
    /// Implements the <see cref="IAuthenticationService" />
    /// </summary>
    /// <seealso cref="IAuthenticationService" />
    public class AuthenticationService : IAuthenticationService
    {
        public Task AuthenticateWithApiKeyAsync(string apiKey)
        {
            throw new NotImplementedException();
        }

        public Task AuthenticateWithClientCredentialsAsync()
        {
            throw new NotImplementedException();
        }

        public Task AuthenticateWithRefreshTokenAsync(string refreshtoken)
        {
            throw new NotImplementedException();
        }

        public Task AuthenticateWithUsernamePasswordAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task CheckForExpiredToken()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTransient(Exception e)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
