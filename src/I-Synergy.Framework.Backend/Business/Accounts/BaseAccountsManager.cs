using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Mapster;
using ISynergy.Extensions;
using ISynergy.Exceptions;
using ISynergy.Models.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Abstractions;
using ISynergy.Entities.Accounts;
using ISynergy.Enumerations;
using ISynergy.Utilities;

namespace ISynergy.Business.Base
{
    public abstract class BaseAccountsManager<TAccount, TUser, TModule, TApiKey, TAccountModule, TDbContext> : BaseEntityManager<TDbContext>
        where TAccount : class, IAccount, new()
        where TUser : class, IUser, new()
        where TModule : class, IModule, new()
        where TApiKey : class, IApiKey, new()
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

        protected BaseAccountsManager(
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

        public async Task<List<AccountFull>> GetAccountsAsync(bool isAdministrator)
        {
            var accounts = Context.Set<TAccount>().AsQueryable();

            if (!isAdministrator) accounts = accounts.Where(q => q.Description != "I-Synergy");

            var result = await accounts
                .OrderBy(o => o.Description)
                .ProjectToType<AccountFull>()
                .ToListAsync()
                ;

            foreach (var account in result.EnsureNotNull())
            {
                foreach (var user in account.Users.EnsureNotNull())
                {
                    user.Roles = new List<Role>();

                    var idUser = await UserManager.FindByIdAsync(user.Id);

                    var roles = await UserManager.GetRolesAsync(idUser);

                    foreach (var role in roles.EnsureNotNull())
                    {
                        var item = await RoleManager.FindByNameAsync(role);
                        user.Roles.Add(item.Adapt<Role>());
                    }
                }
            }

            return result;
        }

        public async Task<int> UpdateAccountAsync(AccountFull e)
        {
            var account = await Context.Set<TAccount>()
                .Where(q => q.Account_Id == e.Account_Id)
                .OrderBy(o => o.Description)
                .SingleAsync();

            Context.Set<TAccount>().Update(e.Adapt(account));

            return await Context
                .SaveChangesAsync();
        }

        public async Task<int> ToggleAccountActivationAsync(Guid id)
        {
            var account = await Context.Set<TAccount>()
                .Where(q => q.Account_Id == id)
                .OrderBy(o => o.Description)
                .SingleAsync();

            account.IsActive = !account.IsActive;

            Context.Set<TAccount>().Update(account);

            return await Context
                .SaveChangesAsync();
        }

        public async Task<bool> ToggleUserLockAsync(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user != null)
            {
                if (await UserManager.IsLockedOutAsync(user))
                {
                    await UserManager.SetLockoutEndDateAsync(user, null);
                    await UserManager.ResetAccessFailedCountAsync(user);
                }
                else
                {
                    await UserManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                }
            }

            return await UserManager.IsLockedOutAsync(user);
        }

        public async Task<int> UpdateUserAsync(UserEdit e)
        {
            var result = 0;

            var user = await UserManager.FindByIdAsync(e.Id);

            if (user.UserName != e.UserName)
            {
                user.UserName = e.UserName;
                user.Email = e.UserName;

                user.NormalizedUserName = e.UserName.ToUpper();
                user.NormalizedEmail = e.UserName.ToUpper();
            }

            user.EmailConfirmed = e.IsConfirmed;

            var status = await UserManager.UpdateAsync(user);

            if (status.Succeeded)
            {
                await UserManager.RemoveFromRolesAsync(user, await UserManager.GetRolesAsync(user));
                await UserManager.AddToRolesAsync(user, e.Roles.Select(s => s.Name));

                result = 1;
            }

            return result;
        }

        public async Task<List<Role>> GetRolesAsync(bool isManager, bool isAdministrator)
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
                .ToListAsync()
                ;

            return result;
        }

        public async Task<bool> CheckIfLicenseIsAvailableAsync(string name)
        {
            Argument.IsNotNull(nameof(name), name);

            return !await Context.Set<TAccount>().Where(q => q.Description.ToLower() == name.ToLower()).AnyAsync();
        }

        public async Task<bool> CheckIfEmailIsAvailableAsync(string email)
        {
            Argument.IsNotNull(nameof(email), email);

            return await UserManager.FindByEmailAsync(email) is null;
        }

        public async Task<IdentityResult> ExternalLoginUserAsync(ExternalLoginInfo info, TUser user)
        {
            var result = await UserManager.CreateAsync(user);

            if (result.Succeeded)
            {
                result = await UserManager.AddLoginAsync(user, info);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                }
            }

            return result;
        }

        public async Task<RegistrationResult> RegisterExternalAsync(TAccount account, TUser user, List<TAccountModule> modules, string password)
        {
            RegistrationResult result = null;

            Context.Set<TAccount>().Add(account);
            Context.Set<TAccountModule>().AddRange(modules);

            await Context.SaveChangesAsync();

            var identityResult = await UserManager.CreateAsync(user, password);

            var adminRole = await RoleManager.FindByNameAsync(Administrator.ToString());

            if (adminRole is null)
            {
                adminRole = new IdentityRole(Administrator.ToString());
                await RoleManager.CreateAsync(adminRole);
                await RoleManager.AddClaimAsync(adminRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.admin_view));
                await RoleManager.AddClaimAsync(adminRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.admin_create));
                await RoleManager.AddClaimAsync(adminRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.admin_update));
                await RoleManager.AddClaimAsync(adminRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.admin_delete));
            }

            if (!await UserManager.IsInRoleAsync(user, adminRole.Name))
            {
                identityResult = await UserManager.AddToRoleAsync(user, adminRole.Name);
            }

            var userRole = await RoleManager.FindByNameAsync(User.ToString());

            if (userRole is null)
            {
                userRole = new IdentityRole(User.ToString());
                identityResult = await RoleManager.CreateAsync(userRole);
                identityResult = await RoleManager.AddClaimAsync(userRole, new Claim(ClaimTypes.PermissionType, AutorizationRoles.user_view));
            }

            if (!await UserManager.IsInRoleAsync(user, userRole.Name))
            {
                identityResult = await UserManager.AddToRoleAsync(user, userRole.Name);
            }

            if (identityResult.Succeeded)
            {
                var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);

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

            var identityResult = await UserManager.CreateAsync(user, password);

            if (identityResult.Succeeded)
            {
                foreach (var role in roles.EnsureNotNull())
                {
                    if (!await UserManager.IsInRoleAsync(user, role.Name)) identityResult = await UserManager.AddToRoleAsync(user, role.Name);
                }
            }

            if (identityResult.Succeeded)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);

                result = new RegistrationResult
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Token = token
                };
            }

            return result;
        }

        public async Task<string> CreateApiKeyAsync(ClaimsPrincipal principal)
        {
            if(Guid.TryParse(principal.GetClaim(ClaimTypes.AccountIdType), out var tenantId))
            {
                var key = new TApiKey
                {
                    ApiKey_Id = Guid.NewGuid(),
                    Tenant_Id = tenantId,
                    Key = SecretUtility.GenerateSecret(),
                    CreatedDate = DateTimeOffset.Now,
                    CreatedBy = principal.Identity.Name
                };

                Context
                    .Set<TApiKey>()
                    .Add(key);

                var result = await Context
                    .SaveChangesAsync()
                    ;

                if (result != 0)
                {
                    return key.Key;
                }
                else
                {
                    throw new InvalidProgramException();
                }
            }

            throw new ClaimNotFoundException();
        }

        public async Task<List<Account>> GetItemsAsync(bool isdeleted = false, int page = 0, int pagesize = int.MaxValue)
        {
            var id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

            var result = await Context.Set<TAccount>()
                .Where(q => q.Account_Id != id && q.IsDeleted == isdeleted)
                .OrderBy(q => q.Description)
                .ToPage(page, pagesize)
                .ProjectToType<Account>()
                .ToListAsync()
                ;

            return result;
        }

        public async Task<Account> GetItemAsync(Guid id)
        {
            var result = await Context.Set<TAccount>()
                .Where(q => q.Account_Id == id)
                .ProjectToType<Account>()
                .SingleOrDefaultAsync()
                ;

            return result;
        }

        public Task<string> GetItemDescriptionAsync(Guid id)
        {
            return Context.Set<TAccount>()
                .Where(q => q.Account_Id == id)
                .Select(r => r.Description)
                .SingleOrDefaultAsync();
        }

        public Task<int> GetLicensedUsersAsync(Guid id)
        {
            return Context.Set<TAccount>().Where(q => q.Account_Id == id).Select(r => r.UsersAllowed).SingleOrDefaultAsync();
        }

        public Task<DateTimeOffset> GetLicenseExpirationAsync(Guid id)
        {
            return Context.Set<TAccount>().Where(q => q.Account_Id == id).Select(r => r.Expiration_Date).SingleOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersAsync(Guid id)
        {
            var users = await Context.Set<TUser>()
                .Where(q => q.Account_Id == id)
                .ProjectToType<User>()
                .ToListAsync()
                ;

            return users;
        }

        public async Task<List<User>> GetAdminUsersAsync(Guid id)
        {
            var users = new List<User>();

            var administrators = await UserManager.GetUsersInRoleAsync(Administrator);

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

        public async Task<int> RemoveUserAsync(string id)
        {
            var item = await UserManager.FindByIdAsync(id);

            if (item != null)
            {
                var result = await UserManager.DeleteAsync(item);

                if (result == IdentityResult.Success)
                {
                    return 1;
                }
            }

            return 0;
        }

        public async Task<int> RemoveAccountAsync(Guid id)
        {
            var result = 0;

            var account = await Context.Set<TAccount>().Where(q => q.Account_Id == id).SingleAsync();

            if (account != null)
            {
                Context.Set<TAccount>().Remove(account);

                result = await Context
                    .SaveChangesAsync()
                    ;
            }

            return result;
        }

        private Task<TUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return UserManager.GetUserAsync(principal);
        }

        public async Task<AuthenticationTicket> AcceptAsync(OpenIdConnectRequest request, ClaimsPrincipal user)
        {
            // Retrieve the profile of the logged in user.
            var userProfile = await GetUserAsync(user);

            if (userProfile is null) return null;

            // Create a new authentication ticket.
            return await CreateTicketAsync(request, userProfile);
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
                        user = await ExchangePasswordGrantTypeAsync(request);
                        break;

                    case GrantTypes.RefreshTokenAndAuthorizationCodeGrantType:
                        user = await ExchangeRefreshTokenGrantTypeAsync(request, info);
                        break;

                    case GrantTypes.ClientCredentialsGrantType:
                        user = await ExchangeClientCredentialsGrantTypeAsync(request, application.ClientId);
                        break;
                }

                account = await GetAccountAsync(user.Account_Id);

                if (account is null)
                {
                    throw new OpenIdConnectException(new OpenIdConnectResponse
                    {
                        Error = Constants.AuthenticationError,
                        ErrorDescription = "EX_ACCOUNT_LICENSE_NOT_VALID"
                    });
                }

                roles = await UserManager.GetRolesAsync(user);
                if (roles is null)
                {
                    throw new OpenIdConnectException(new OpenIdConnectResponse
                    {
                        Error = Constants.AuthenticationError,
                        ErrorDescription = "EX_ACCOUNT_LOGIN_FAILED"
                    });
                }

                if (!account.IsActive)
                {
                    throw new OpenIdConnectException(new OpenIdConnectResponse
                    {
                        Error = Constants.AuthenticationError,
                        ErrorDescription = "EX_ACCOUNT_NOT_ACTIVE"
                    });
                }

                if (account.Expiration_Date < DateTime.Now)
                {
                    throw new OpenIdConnectException(new OpenIdConnectResponse
                    {
                        Error = Constants.AuthenticationError,
                        ErrorDescription = "EX_ACCOUNT_LICENSE_EXPIRED"
                    });
                }

                // Create a new authentication ticket, but reuse the properties stored in the
                // authorization code/refresh token, including the scopes originally granted.
                result = await CreateTicketAsync(request, user, account, roles, info?.Properties);

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
            var user = await UserManager.FindByNameAsync(request.Username);

            if (user is null)
            {
                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Constants.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOGIN_FAILED"
                });
            }

            // Validate the username/password parameters and ensure the account is not locked out.
            var result = await SignInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (UserManager.SupportsUserLockout)
                {
                    await UserManager.SetLockoutEndDateAsync(user, null);
                }

                await UserManager.ResetAccessFailedCountAsync(user);

                return user;
            }
            else if (result.IsLockedOut)
            {
                if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user))
                {
                    await UserManager.AccessFailedAsync(user);
                }

                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Constants.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOCKED"
                });
            }
            else if (result.IsNotAllowed)
            {
                if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user))
                {
                    await UserManager.AccessFailedAsync(user);
                }

                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Constants.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOCKED"
                });
            }
            else if (result.RequiresTwoFactor)
            {
                if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user))
                {
                    await UserManager.AccessFailedAsync(user);
                }

                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Constants.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_2FACTOR"
                });
            }
            else
            {
                if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user))
                {
                    await UserManager.AccessFailedAsync(user);
                }

                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Constants.AuthenticationError,
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
            var user = await UserManager.GetUserAsync(info.Principal);

            if (user is null)
            {
                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Constants.AuthenticationError,
                    ErrorDescription = "EX_ACCOUNT_LOGIN_FAILED"
                });
            }

            // Ensure the user is still allowed to sign in.
            if (!await SignInManager.CanSignInAsync(user))
            {
                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Constants.AuthenticationError,
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
            var user = await UserManager.FindByNameAsync(clientid);

            if (user is null)
            {
                throw new OpenIdConnectException(new OpenIdConnectResponse
                {
                    Error = Constants.AuthenticationError,
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
                principal = await SignInManager.CreateUserPrincipalAsync(user);
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

            var result = new ClaimsIdentity(
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
