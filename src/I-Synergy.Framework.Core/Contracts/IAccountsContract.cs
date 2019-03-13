using ISynergy.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Contracts.Accounts
{
    public interface IAccountsContract
    {
        Task<bool> ForgotPasswordExternal(string email);
        Task<bool> CheckIfEmailIsAvailableAsync(string email);
        Task<bool> CheckIfLicenseIsAvailableAsync(string name);
        Task<bool> RegisterExternal(RegistrationData e);
        Task<bool> AddUserAsync(UserAdd e);
    }

    public interface IAccountsManagerContract : IAccountsContract
    {
        Task<List<AccountFull>> GetAccountsAsync(CancellationToken cancellationToken = default);
        Task<int> UpdateAccountAsync(AccountFull e);
        Task<int> UpdateUserAsync(UserEdit e);
        Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default);
        Task<int> RemoveUserAsync(string id);
        Task<int> RemoveAccountAsync(Guid id);
        Task<int> ToggleAccountActivationAsync(Guid id);
        Task<bool> ToggleUserLockAsync(string id);
    }
}
