using ISynergy.Contracts.Accounts;
using System;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IAuthenticationService :
        IAccountsContract,
        IAccountsManagerContract
    {
        Task CheckForExpiredToken();
        Task<bool> IsTransient(Exception e);
        Task AuthenticateWithTokenAsync(string username, string password);
        Task AuthenticateWithClientCredentialsAsync();
        Task AuthenticateWithRefreshTokenAsync(string refreshtoken);
    }
}
