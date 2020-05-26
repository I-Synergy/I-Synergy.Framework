using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IAuthenticationService
    {
        Task CheckForExpiredToken();
        Task<bool> IsTransient(Exception e);
        Task AuthenticateWithUsernamePasswordAsync(string username, string password);
        Task AuthenticateWithClientCredentialsAsync();
        Task AuthenticateWithRefreshTokenAsync(string refreshtoken);
    }
}
