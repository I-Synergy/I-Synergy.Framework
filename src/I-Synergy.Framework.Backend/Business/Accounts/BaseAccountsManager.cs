using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Mapster;
using ISynergy.Business.Base;
using ISynergy.Common;
using ISynergy.Extensions;
using ISynergy.Utilities;
using ISynergy.Exceptions;
using ISynergy.Library;
using ISynergy.Models.Accounts;
using ISynergy.Models.General;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Services;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Abstractions;
using ISynergy.Entities.Accounts;

namespace ISynergy.Business.Base
{
    public abstract class BaseAccountsManager<TAccount, TUser, TModule, TAccountModule, TDbContext> : BaseEntityManager<TDbContext>
        where TAccount : class, IAccount, new()
        where TUser : class, IUser, new()
        where TModule : class, IModule, new()
        where TAccountModule : class, IAccountModule, new()
        where TDbContext : DbContext
    {
        public const int TrialLength = 30;

        private const string Guest = "Guest";
        private const string User = "User";
        private const string Administrator = "Administrator";
        private const string License_Manager = "License_Manager";
        private const string License_Administrator = "License_Administrator";

        public UserManager<TUser> UserManager { get; }
        public SignInManager<TUser> SignInManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        public BaseAccountsManager(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            TDbContext context,
            ILoggerFactory loggerFactory)
            : base(context, loggerFactory)
        {
            Argument.IsNotNull(nameof(context), context);

            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public async Task<List<AccountFull>> GetAccountsAsync(bool isAdministrator, CancellationToken cancellationToken = default)
        {
            var accounts = Context.Set<TAccount>().AsQueryable();

            if (!isAdministrator) accounts = accounts.Where(q => q.Description != "I-Synergy");

            var result = await accounts
                .OrderBy(o => o.Description)
                .ProjectToType<AccountFull>()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (AccountFull account in result.EnsureNotNull())
            {
                foreach (UserFull user in account.Users.EnsureNotNull())
                {
                    user.Roles = new List<Role>();

                    var idUser = await UserManager.FindByIdAsync(user.Id).ConfigureAwait(false);

                    IList<string> roles = await UserManager.GetRolesAsync(idUser).ConfigureAwait(false);

                    foreach (string role in roles.EnsureNotNull())
                    {
                        var item = await RoleManager.FindByNameAsync(role).ConfigureAwait(false);
                        user.Roles.Add(item.Adapt<Role>());
                    }
                }
            }

            return result;
        }

        public async Task<int> UpdateAccountAsync(AccountFull e, CancellationToken cancellationToken = default)
        {
            int result = 0;

            var account = await Context.Set<TAccount>()
                .Where(q => q.Account_Id == e.Account_Id)
                .OrderBy(o => o.Description)
                .SingleAsync(cancellationToken)
                .ConfigureAwait(false);

            Context.Set<TAccount>().Update(e.Adapt(account));

            result = await Context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<int> ToggleAccountActivationAsync(Guid id, CancellationToken cancellationToken = default)
        {
            int result = 0;

            var account = await Context.Set<TAccount>()
                .Where(q => q.Account_Id == id)
                .OrderBy(o => o.Description)
                .SingleAsync(cancellationToken)
                .ConfigureAwait(false);

            account.IsActive = !account.IsActive;

            Context.Set<TAccount>().Update(account);

            result = await Context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<bool> ToggleUserLockAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await UserManager.FindByIdAsync(id).ConfigureAwait(false);

            if (user != null)
            {
                if (await UserManager.IsLockedOutAsync(user).ConfigureAwait(false))
                {
                    await UserManager.SetLockoutEndDateAsync(user, null).ConfigureAwait(false);
                    await UserManager.ResetAccessFailedCountAsync(user).ConfigureAwait(false);
                }
                else
                {
                    await UserManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue).ConfigureAwait(false);
                }
            }

            return await UserManager.IsLockedOutAsync(user).ConfigureAwait(false);
        }

        public async Task<int> UpdateUserAsync(UserEdit e, CancellationToken cancellationToken = default)
        {
            int result = 0;

            var user = await UserManager.FindByIdAsync(e.Id).ConfigureAwait(false);

            if (user.UserName != e.UserName)
            {
                user.UserName = e.UserName;
                user.Email = e.UserName;

                user.NormalizedUserName = e.UserName.ToUpper();
                user.NormalizedEmail = e.UserName.ToUpper();
            }

            user.EmailConfirmed = e.IsConfirmed;

            var status = await UserManager.UpdateAsync(user).ConfigureAwait(false);

            if (status.Succeeded)
            {
                await UserManager.RemoveFromRolesAsync(user, await UserManager.GetRolesAsync(user).ConfigureAwait(false)).ConfigureAwait(false);
                await UserManager.AddToRolesAsync(user, e.Roles.Select(s => s.Name)).ConfigureAwait(false);

                result = 1;
            }

            return result;
        }

        public async Task<List<Role>> GetRolesAsync(bool isManager, bool isAdministrator, CancellationToken cancellationToken = default)
        {
            var roles = Context.Set<IdentityRole>().AsQueryable();

            if (!isManager)
            {
                roles = roles.Where(q => q.NormalizedName != License_Manager.ToUpper());
            }

            if (!isAdministrator)
            {
                roles = roles.Where(q => q.NormalizedName != License_Administrator.ToUpper());
            }

            var result = await roles
                .OrderBy(o => o.Name)
                .ProjectToType<Role>()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<bool> CheckIfLicenseIsAvailableAsync(string name)
        {
            Argument.IsNotNull(nameof(name), name);

            return !await Context.Set<TAccount>().Where(q => q.Description.ToLower() == name.ToLower()).AnyAsync().ConfigureAwait(false);
        }

        public async Task<bool> CheckIfEmailIsAvailableAsync(string email)
        {
            Argument.IsNotNull(nameof(email), email);

            return await UserManager.FindByEmailAsync(email).ConfigureAwait(false) is null;
        }

        public async Task<IdentityResult> ExternalLoginUserAsync(ExternalLoginInfo info, TUser user)
        {
            var result = await UserManager.CreateAsync(user).ConfigureAwait(false);

            if (result.Succeeded)
            {
                result = await UserManager.AddLoginAsync(user, info).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                }
            }

            return result;
        }

        public async Task<RegistrationResult> RegisterExternalAsync(TAccount account, TUser user, List<TAccountModule> modules, string password, string input)
        {
            RegistrationResult result = null;

            Context.Set<TAccount>().Add(account);
            Context.Set<TAccountModule>().AddRange(modules);

            await Context.SaveChangesAsync().ConfigureAwait(false);

            var identityResult = await UserManager.CreateAsync(user, password).ConfigureAwait(false);

            var adminRole = await RoleManager.FindByNameAsync(Administrator.ToString()).ConfigureAwait(false);

            if (adminRole is null)
            {
                adminRole = new IdentityRole(Administrator.ToString());
                await RoleManager.CreateAsync(adminRole).ConfigureAwait(false);
                await RoleManager.AddClaimAsync(adminRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.admin_view)).ConfigureAwait(false);
                await RoleManager.AddClaimAsync(adminRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.admin_create)).ConfigureAwait(false);
                await RoleManager.AddClaimAsync(adminRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.admin_update)).ConfigureAwait(false);
                await RoleManager.AddClaimAsync(adminRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.admin_delete)).ConfigureAwait(false);
            }

            if (!await UserManager.IsInRoleAsync(user, adminRole.Name).ConfigureAwait(false))
            {
                identityResult = await UserManager.AddToRoleAsync(user, adminRole.Name).ConfigureAwait(false);
            }

            var userRole = await RoleManager.FindByNameAsync(User.ToString()).ConfigureAwait(false);

            if (userRole is null)
            {
                userRole = new IdentityRole(User.ToString());
                identityResult = await RoleManager.CreateAsync(userRole).ConfigureAwait(false);
                identityResult = await RoleManager.AddClaimAsync(userRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.user_view)).ConfigureAwait(false);
            }

            if (!await UserManager.IsInRoleAsync(user, userRole.Name).ConfigureAwait(false))
            {
                identityResult = await UserManager.AddToRoleAsync(user, userRole.Name).ConfigureAwait(false);
            }

            if (identityResult.Succeeded)
            {
                string token = await UserManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

                result = new RegistrationResult
                {
                    UserId = user.Id,
                    Account = account.Description,
                    Email = user.Email,
                    Token = token
                };
            }

            return result;
        }

        public async Task<RegistrationResult> AddUserAsync(TUser user, string password, List<Role> roles)
        {
            RegistrationResult result = null;

            var identityResult = await UserManager.CreateAsync(user, password).ConfigureAwait(false);

            if (identityResult.Succeeded)
            {
                foreach (var role in roles.EnsureNotNull())
                {
                    if (!await UserManager.IsInRoleAsync(user, role.Name).ConfigureAwait(false)) identityResult = await UserManager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);
                }
            }

            if (identityResult.Succeeded)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                string token = await UserManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

                result = new RegistrationResult
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Token = token
                };
            }

            return result;
        }

        //public async Task<User> GetCurrentUserAsync()
        //{
        //    ISynergy.TUser result = await UserManager.GetUserAsync(User);
        //    return ;
        //}

        public async Task<List<Account>> GetItemsAsync(bool isdeleted = false, int page = 1, int pagesize = int.MaxValue, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

            var result = await Context.Set<TAccount>()
                .Where(q => q.Account_Id != id && q.IsDeleted == isdeleted)
                .OrderBy(q => q.Description)
                .ToPage(page, pagesize)
                .ProjectToType<Account>()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<Account> GetItemAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await Context.Set<TAccount>()
                .Where(q => q.Account_Id == id)
                .ProjectToType<Account>()
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<string> GetItemDescriptionAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Context.Set<TAccount>()
                .Where(q => q.Account_Id == id)
                .Select(r => r.Description)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<int> GetLicensedUsersAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Context.Set<TAccount>().Where(q => q.Account_Id == id).Select(r => r.UsersAllowed).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<DateTimeOffset> GetLicenseExpirationAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Context.Set<TAccount>().Where(q => q.Account_Id == id).Select(r => r.Expiration_Date).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<List<User>> GetUsersAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var users = await Context.Set<TUser>()
                .Where(q => q.Account_Id == id)
                .ProjectToType<User>()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return users;
        }

        public async Task<List<User>> GetAdminUsersAsync(Guid id, CancellationToken cancellationToken = default)
        {
            List<User> users = new List<User>();

            var administrators = await UserManager.GetUsersInRoleAsync(Administrator).ConfigureAwait(false);

            if (administrators != null && administrators.Count > 0)
            {
                users = administrators
                    .Join(Context.Set<TAccount>(),
                        User => User.Account_Id,
                        Account => Account.Account_Id,
                        (user, account) =>
                        new
                        {
                            account.Account_Id,
                            account.IsActive,
                            user
                        })
                .Where(q => q.Account_Id == id && q.IsActive)
                .Select(s => s.user.Adapt<User>())
                .AsQueryable()
                .ToList();
            }

            return users;
        }

        public async Task<int> RemoveUserAsync(string id, CancellationToken cancellationToken = default)
        {
            var item = await UserManager.FindByIdAsync(id).ConfigureAwait(false);

            if (item != null)
            {
                var result = await UserManager.DeleteAsync(item).ConfigureAwait(false);

                if (result == IdentityResult.Success)
                {
                    return 1;
                }
            }

            return 0;
        }

        public async Task<int> RemoveAccountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            int result = 0;

            var account = await Context.Set<TAccount>().Where(q => q.Account_Id == id).SingleAsync(cancellationToken).ConfigureAwait(false);

            if (account != null)
            {
                Context.Set<TAccount>().Remove(account);

                result = await Context
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            return result;
        }


        private Task<TUser> GetUsersAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
        {
            return UserManager.GetUserAsync(principal);
        }

        public async Task<AuthenticationTicket> AcceptAsync(OpenIdConnectRequest request, ClaimsPrincipal user)
        {
            // Retrieve the profile of the logged in user.
            var userProfile = await GetUsersAsync(user).ConfigureAwait(false);

            if (userProfile is null) return null;

            // Create a new authentication ticket.
            return await CreateTicketAsync(request, userProfile).ConfigureAwait(false);
        }

        public async Task<AuthenticationTicket> ExchangeAsync(OpenIdConnectRequest request, GrantTypes grantTypes, AuthenticateResult info = null, OpenIddictApplication application = null)
        {
            AuthenticationTicket result = null;

            if (request.IsPasswordGrantType() || request.IsClientCredentialsGrantType() || request.IsRefreshTokenGrantType())
            {
                TUser user = null;
                TAccount account = null;
                IList<string> roles = null;

                switch (grantTypes)
                {
                    case GrantTypes.PasswordGrantType:
                        user = await ExchangePasswordGrantTypeAsync(request).ConfigureAwait(false);
                        break;

                    case GrantTypes.RefreshTokenAndAuthorizationCodeGrantType:
                        user = await ExchangeRefreshTokenGrantTypeAsync(request, info).ConfigureAwait(false);
                        break;

                    case GrantTypes.ClientCredentialsGrantType:
                        user = await ExchangeClientCredentialsGrantTypeAsync(request, application.ClientId).ConfigureAwait(false);
                        break;
                }

                account = await GetAccountAsync(user.Account_Id).ConfigureAwait(false);

                if (account is null)
                {
                    throw new OpenIdConnectException(new OpenIdConnectResponse
                    {
                        Error = Globals.AuthenticationError,
                        ErrorDescription = "EX_ACCOUNT_LICENSE_NOT_VALID"
                    });
                }

                roles = await UserManager.GetRolesAsync(user).ConfigureAwait(false);
                if (roles is null)
                {
                    throw new OpenIdConnectException(new OpenIdConnectResponse
                    {
                        Error = Globals.AuthenticationError,
                        ErrorDescription = "EX_ACCOUNT_LOGIN_FAILED"
                    });
                }

                if (!account.IsActive)
                {
                    throw new OpenIdConnectException(new OpenIdConnectResponse
                    {
                        Error = Globals.AuthenticationError,
                        ErrorDescription = "EX_ACCOUNT_NOT_ACTIVE"
                    });
                }

                if (account.Expiration_Date < DateTime.Now)
                {
                    throw new OpenIdConnectException(new OpenIdConnectResponse
                    {
                        Error = Globals.AuthenticationError,
                        ErrorDescription = "EX_ACCOUNT_LICENSE_EXPIRED"
                    });
                }

                // Create a new authentication ticket, but reuse the properties stored in the
                // authorization code/refresh token, including the scopes originally granted.
                result = await CreateTicketAsync(request, user, account, roles, info?.Properties).ConfigureAwait(false);

                // Create a new push message to notify subscriber of new loggedin user 
                // await PubNubService.PublishAsync(
                //    account.Account_Id.ToString(),
                //    nameof(PubNubEventNames.Connected),
                //    new Dictionary<string, object> { { PubNubEventNames.Connected, user.UserName } });
            }

            return result;
        }

        private async Task<TUser> ExchangePasswordGrantTypeAsync(OpenIdConnectRequest request)
        {
            TUser user = await UserManager.FindByNameAsync(request.Username).ConfigureAwait(false);

            if (user is null)
            {
                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Globals.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOGIN_FAILED"
                });
            }

            // Validate the username/password parameters and ensure the account is not locked out.
            var result = await SignInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true).ConfigureAwait(false);

            if (result.Succeeded)
            {
                if (UserManager.SupportsUserLockout)
                {
                    await UserManager.SetLockoutEndDateAsync(user, null).ConfigureAwait(false);
                }

                await UserManager.ResetAccessFailedCountAsync(user).ConfigureAwait(false);

                return user;
            }
            else if (result.IsLockedOut)
            {
                if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user).ConfigureAwait(false))
                {
                    await UserManager.AccessFailedAsync(user).ConfigureAwait(false);
                }

                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Globals.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOCKED"
                });
            }
            else if (result.IsNotAllowed)
            {
                if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user).ConfigureAwait(false))
                {
                    await UserManager.AccessFailedAsync(user).ConfigureAwait(false);
                }

                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Globals.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOCKED"
                });
            }
            else if (result.RequiresTwoFactor)
            {
                if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user).ConfigureAwait(false))
                {
                    await UserManager.AccessFailedAsync(user).ConfigureAwait(false);
                }

                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Globals.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_2FACTOR"
                });
            }
            else
            {
                if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user).ConfigureAwait(false))
                {
                    await UserManager.AccessFailedAsync(user).ConfigureAwait(false);
                }

                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Globals.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOGIN_FAILED"
                });
            }
        }

        private async Task<TUser> ExchangeRefreshTokenGrantTypeAsync(OpenIdConnectRequest request, AuthenticateResult info)
        {
            // Retrieve the user profile corresponding to the authorization code/refresh token.
            // Note: if you want to automatically invalidate the authorization code/refresh token
            // when the user password/roles change, use the following line instead:
            // var user = Manager.SignInManager.ValidateSecurityStampAsync(info.Principal);
            TUser user = await UserManager.GetUserAsync(info.Principal).ConfigureAwait(false);

            if (user is null)
            {
                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Globals.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOGIN_FAILED"
                });
            }

            // Ensure the user is still allowed to sign in.
            if (!await SignInManager.CanSignInAsync(user).ConfigureAwait(false))
            {
                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Globals.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOCKED"
                });
            }

            return user;
        }

        private async Task<TUser> ExchangeClientCredentialsGrantTypeAsync(OpenIdConnectRequest request, string clientid)
        {
            // Retrieve the user profile corresponding to the authorization code/refresh token.
            // Note: if you want to automatically invalidate the authorization code/refresh token
            // when the user password/roles change, use the following line instead:
            // var user = Manager.SignInManager.ValidateSecurityStampAsync(info.Principal);
            TUser user = await UserManager.FindByNameAsync(clientid).ConfigureAwait(false);

            if (user is null)
            {
                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Globals.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOGIN_FAILED"
                });
            }

            return user;
        }

        protected async Task<AuthenticationTicket> CreateTicketAsync(
            OpenIdConnectRequest request,
            TUser user,
            TAccount account = null,
            IList<string> roles = null,
            AuthenticationProperties properties = null)
        {
            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            ClaimsPrincipal principal = null;

            if (account != null)
            {
                var identity = CreateIdentity(OpenIdConnectServerDefaults.AuthenticationScheme, user, account, roles);

                principal = new ClaimsPrincipal(identity);
            }
            else
            {
                principal = await SignInManager.CreateUserPrincipalAsync(user).ConfigureAwait(false);
            }

            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(principal, properties, OpenIdConnectServerDefaults.AuthenticationScheme);

            if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
            {
                // Set the list of scopes granted to the client application.
                // Note: the offline_access scope must be granted
                // to allow OpenIddict to return a refresh token.
                ticket.SetScopes(new[]
                {
                    OpenIdConnectConstants.Scopes.OpenId,
                    OpenIdConnectConstants.Scopes.Email,
                    OpenIdConnectConstants.Scopes.Profile,
                    OpenIdConnectConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles
                }.Intersect(request.GetScopes()));
            }

            ticket.SetResources("i-synergy-api");

            return ticket;
        }

        protected virtual ClaimsIdentity CreateIdentity(
            string authenticationType,
            TUser user,
            TAccount account,
            IEnumerable<string> roles)
        {
            Argument.IsNotNull(nameof(user), user);
            Argument.IsNotNull(nameof(account), account);

            ClaimsIdentity result = new ClaimsIdentity(
                authenticationType,
                OpenIdConnectConstants.Claims.Name,
                OpenIdConnectConstants.Claims.Role);

            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            // Note: the name identifier is always included in both identity and
            // access tokens, even if an explicit destination is not specified.
            result.AddClaim(OpenIdConnectConstants.Claims.Subject, user.Id.ToString());
            result.AddClaim(OpenIdConnectConstants.Claims.Name, user.UserName, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);

            // When adding custom claims, you MUST specify one or more destinations.
            // When access_token and id_token are specified, the claim will be serialized in both tokens.
            // If only access_token is specified, the language claim won't be added in the identity token.

            result.AddClaim(ClaimTypes.AccountIdType, account.Account_Id.ToString(), OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            result.AddClaim(ClaimTypes.AccountDescriptionType, account.Description.ToString(), OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            result.AddClaim(ClaimTypes.TimeZoneType, account.TimeZoneId, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            result.AddClaim(ClaimTypes.UserNameType, user.UserName, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            result.AddClaim(ClaimTypes.UserIdType, user.Id.ToString(), OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);

            roles?.ForEach(role =>
            {
                if (!string.IsNullOrWhiteSpace(role))
                    result.AddClaim(OpenIdConnectConstants.Claims.Role, role, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            });

            GetModules(account)?.ForEach(module =>
            {
                result.AddClaim(ClaimTypes.ModulesType, module.Name, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            });

            result.AddClaim(ClaimTypes.LicenseExprationType, account.Expiration_Date.ToString(CultureInfo.InvariantCulture), OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            result.AddClaim(ClaimTypes.LicenseUsersType, account.UsersAllowed.ToString(), OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            result.AddClaim(ClaimTypes.SecurityStampType, user.SecurityStamp, OpenIdConnectConstants.Destinations.AccessToken);

            return result;
        }

        protected abstract Task<TAccount> GetAccountAsync(Guid id);
        protected abstract List<TModule> GetModules(TAccount account);
    }
}
