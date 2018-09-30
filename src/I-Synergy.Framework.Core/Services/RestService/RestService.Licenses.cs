using ISynergy.Contracts.Accounts;
using ISynergy.Models.Accounts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public abstract partial class RestServiceBase : IAccountsManagerContract
    {
        public Task<List<AccountFull>> GetAccountsAsync(CancellationToken cancellationToken = default) =>
            GetAccountJsonAsync<List<AccountFull>>(new object[] { ControllerPaths.Accounts });

        public Task<int> UpdateAccountAsync(AccountFull e, CancellationToken cancellationToken = default) =>
            PutAccountJsonAsync<int>(new object[] { ControllerPaths.Accounts }, e);

        public async Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            List<Role> result = new List<Role>();

            var roles = await GetAccountJsonAsync<List<Role>>(new object[] { ControllerPaths.Roles });

            foreach (var item in roles.EnsureNotNull())
            {
                item.Description = LanguageService.GetString($"Role_{item.Name}");
                result.Add(item);
            }

            return result;
        }

        public Task<int> RemoveUserAsync(string id, CancellationToken cancellationToken = default) =>
            DeleteAccountJsonAsync(new object[] { ControllerPaths.Users, id });

        public Task<int> RemoveAccountAsync(Guid id, CancellationToken cancellationToken = default) =>
            DeleteAccountJsonAsync(new object[] { ControllerPaths.Accounts, id });

        public Task<int> ToggleAccountActivationAsync(Guid id, CancellationToken cancellationToken = default) =>
            PutAccountJsonAsync<int>(new object[] { ControllerPaths.Accounts, id }, null);

        public Task<bool> ToggleUserLockAsync(string id, CancellationToken cancellationToken = default) =>
            PutAccountJsonAsync<bool>(new object[] { ControllerPaths.Users, id }, null);

        public Task<int> UpdateUserAsync(UserEdit e, CancellationToken cancellationToken = default) =>
            PutAccountJsonAsync<int>(new object[] { ControllerPaths.Users }, e);
    }
}
