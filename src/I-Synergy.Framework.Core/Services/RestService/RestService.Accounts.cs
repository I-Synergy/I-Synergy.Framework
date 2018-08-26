using ISynergy.Contracts.Accounts;
using ISynergy.Models.Accounts;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public abstract partial class RestServiceBase : IAccountsContract
    {
        public Task<bool> CheckIfEmailIsAvailableAsync(string email) =>
            GetAccountJsonAsync<bool>(new object[] { ControllerPaths.Check, ControllerPaths.Email, email }, IsAnonymous: true);

        public Task<bool> CheckIfLicenseIsAvailableAsync(string name) =>
            GetAccountJsonAsync<bool>(new object[] { ControllerPaths.Check, ControllerPaths.License, name }, IsAnonymous: true);

        public Task<bool> ForgotPasswordExternal(string email) =>
            GetAccountJsonAsync<bool>(new object[] { ControllerPaths.ForgotPassword, email }, IsAnonymous: true);

        public Task<bool> RegisterExternal(RegistrationData e) =>
            PostAccountJsonAsync(new object[] { ControllerPaths.RegisterExternal }, e, IsAnonymous: true);

        public Task<bool> AddUserAsync(UserAdd e) =>
            PostAccountJsonAsync(new object[] { ControllerPaths.Users }, e);

    }
}
