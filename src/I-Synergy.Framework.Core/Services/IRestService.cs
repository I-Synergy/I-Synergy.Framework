using Flurl.Http;
using ISynergy.Contracts.Accounts;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IBaseRestService :
        IAccountsContract,
        IAccountsManagerContract
    {
        IFlurlClient RestClient { get; }

        Task AuthenticateWithTokenAsync(string username, string password);
        Task AuthenticateWithClientCredentialsAsync();
        Task AuthenticateWithRefreshTokenAsync(string refreshtoken);
    }
}