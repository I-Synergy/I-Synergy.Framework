using Flurl;
using Flurl.Http;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Exceptions;
using ISynergy.Models.Accounts;
using ISynergy.Models.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public IContext Context { get; }
        public IFlurlClient Client { get; }
        public IBusyService BusyService { get; }
        public IDialogService DialogService { get; }
        public ILanguageService LanguageService { get; }
        public IBaseSettingsService SettingsService { get; }
        public ITelemetryService TelemetryService { get; }

        public AuthenticationService(
            IContext context,
            IFlurlClient client,
            IBusyService busy,
            ILanguageService language,
            IDialogService dialogService,
            IBaseSettingsService settings,
            ITelemetryService telemetry)
        {
            Context = context;
            Client = client;
            BusyService = busy;
            DialogService = dialogService;
            LanguageService = language;
            SettingsService = settings;
            TelemetryService = telemetry;
        }

        public async Task CheckForExpiredToken()
        {
            if (DateTime.Now.CompareTo(Context.CurrentProfile?.TokenExpiration) >= 0 && Context.CurrentProfile?.Token != null)
            {
                await AuthenticateWithRefreshTokenAsync(Context.CurrentProfile?.Token.Refresh_Token);
            }
        }

        public async Task<bool> IsTransient(Exception e)
        {
            bool result = false;

            // Determine if the exception is transient.
            // In some cases this is as simple as checking the exception type, in other
            // cases it might be necessary to inspect other properties of the exception.
            if (e is FlurlHttpException ex)
            {
                //Check if access-token is expired
                if (ex.Call.HttpStatus == System.Net.HttpStatusCode.Unauthorized)
                {
                    await AuthenticateWithRefreshTokenAsync(Context.CurrentProfile?.Token.Refresh_Token);

                    if (Context.CurrentProfile?.Token is null)
                    {
                        result = false;
                        Messenger.Default.Send(new AuthenticateUserMessageRequest(this, true));
                    }
                    else
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        protected async Task<T> GetAccountJsonAsync<T>(object[] segments, object queryparameters = null, bool IsAnonymous = false, CancellationToken cancellationToken = default)
        {
            T result = default;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await CheckForExpiredToken();

                    // Call external service.
                    var url = new Url(Context.AccountUrl)
                        .AppendPathSegments(segments)
                        .SetQueryParams(queryparameters)
                        .WithClient(Client);

                    if (!IsAnonymous) url.WithOAuthBearerToken(Context.CurrentProfile?.Token.Access_Token);

                    result = await url.GetJsonAsync<T>(cancellationToken);

                    // Return or break.
                    break;
                }
                catch (Exception e)
                {
                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    // Determine whether to retry the operation, as well as how
                    // long to wait, based on the retry strategy.
                    if (currentRetry > Constants.RestRetryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential delay here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(TimeSpan.FromSeconds(Constants.RestRetryDelayInSeconds));
            }

            return result;
        }

        public async Task AuthenticateWithTokenAsync(string username, string password)
        {
            Argument.IsNotNullOrEmpty(nameof(username), username);
            Argument.IsNotNullOrEmpty(nameof(password), password);

            try
            {
                var token = await new Url(Context.TokenUrl)
                        .WithClient(Client)
                        .PostUrlEncodedAsync(new Grant
                        {
                            Grant_Type = Grant_Types.password,
                            Username = username,
                            Password = password,
                            Client_Id = Context.Client_Id,
                            Client_Secret = Context.Client_Secret,
                            Scope = Grant_Scopes.password
                        })
                        .ReceiveJson<Token>();

                if (token != null)
                {
                    Context.CurrentProfile = new Profile
                    {
                        Username = username,
                        Token = token
                    };
                }
            }
            catch (FlurlHttpException e)
            {
                await HandleFlurlExceptionAsync(e);
            }
        }

        public async Task AuthenticateWithClientCredentialsAsync()
        {
            try
            {
                Context.CurrentProfile = new Profile
                {
                    Token = await new Url(Context.TokenUrl)
                        .WithClient(Client)
                        .PostUrlEncodedAsync(new Grant
                        {
                            Grant_Type = Grant_Types.client_credentials,
                            Client_Id = Context.Client_Id,
                            Client_Secret = Context.Client_Secret,
                            Scope = Grant_Scopes.client_credentials
                        })
                        .ReceiveJson<Token>()
                };
            }
            catch (FlurlHttpException e)
            {
                await HandleFlurlExceptionAsync(e);
            }
        }

        /// In the previous betas, OpenIddict used a non-configurable mode codenamed "rolling tokens": every time a refresh token was sent as part of a grant_type=refresh_token
        /// request, it was automatically revoked and a new single-use refresh token was generated and returned to the client application.
        /// This approach was great from a security perspective but had a few downsides.For instance, it didn't play well with heavily distributed client applications
        /// like MVC apps implementing transparent access token renewal (e.g using Microsoft's OIDC client middleware). In such scenario, if two refresh tokens requests
        /// were simultaneously sent with the same refresh token, one of them would be automatically rejected as the refresh token would be already marked as "redeemed"
        /// when handling the second request.
        ///
        /// The previous default behavior is still supported but is now an opt-in option. To enable it, call options.UseRollingTokens() from the OpenIddict configuration
        /// delegate, in ConfigureServices().
        ///
        public async Task AuthenticateWithRefreshTokenAsync(string refreshtoken)
        {
            try
            {
                Context.CurrentProfile = new Profile
                {
                    Token = await new Url(Context.TokenUrl)
                        .WithClient(Client)
                        .PostUrlEncodedAsync(new Grant
                        {
                            Grant_Type = Grant_Types.refresh_token,
                            Refresh_Token = refreshtoken,
                            Client_Id = Context.Client_Id,
                            Client_Secret = Context.Client_Secret
                        })
                        .ReceiveJson<Token>()
                };
            }
            catch (Flurl.Http.FlurlHttpException e)
            {
                if (e.Call.HttpStatus == System.Net.HttpStatusCode.BadRequest)
                {
                    ApiException ex = await e.GetResponseJsonAsync<ApiException>();

                    if (ex != null)
                    {
                        await DialogService.ShowErrorAsync(
                                LanguageService.GetString("EX_ACCOUNT_LOGIN_FAILED"));
                    }
                }

                Messenger.Default.Send(new AuthenticateUserMessageRequest(this, true));
            }
        }

        private async Task HandleFlurlExceptionAsync(FlurlHttpException e)
        {
            if (e.Call.HttpStatus == System.Net.HttpStatusCode.BadRequest)
            {
                ApiException api_ex = await e.GetResponseJsonAsync<ApiException>();

                if (api_ex != null)
                {
                    if (api_ex.Error == Constants.InvalidGrantError)
                    {
                        await DialogService.ShowErrorAsync(
                            LanguageService.GetString("EX_ACCOUNT_LOGIN_FAILED"));
                    }
                    else if (api_ex.Error == Constants.AuthenticationError || api_ex.Error == Constants.UnauthorizedClientError)
                    {
                        await DialogService.ShowErrorAsync(
                            LanguageService.GetString(api_ex.Error_Description));
                    }
                    else
                    {
                        await DialogService.ShowErrorAsync(
                            LanguageService.GetString("EX_ACCOUNT_OTHER"));
                    }
                }
            }
            else
            {
                await DialogService.ShowErrorAsync(
                            LanguageService.GetString("EX_ACCOUNT_OTHER"));
            }
        }

        public async Task<T> PutAccountJsonAsync<T>(object[] segments, object data)
        {
            T result = default;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await CheckForExpiredToken();

                    // Call external service.
                    result = await new Url(Context.AccountUrl)
                        .AppendPathSegments(segments)
                        .WithClient(Client)
                        .WithOAuthBearerToken(Context.CurrentProfile?.Token.Access_Token)
                        .PutJsonAsync(data)
                        .ReceiveJson<T>();

                    // Return or break.
                    break;
                }
                catch (Exception e)
                {
                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    // Determine whether to retry the operation, as well as how
                    // long to wait, based on the retry strategy.
                    if (currentRetry > Constants.RestRetryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential TimeSpan.FromSeconds(Constants.RestRetryDelayInSeconds) here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(TimeSpan.FromSeconds(Constants.RestRetryDelayInSeconds));
            }

            return result;
        }

        public async Task<T> PostAccountJsonAsync<T>(object[] segments, object data, bool IsAnonymous = false)
        {
            T result = default;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await CheckForExpiredToken();

                    // Call external service.
                    var url = new Url(Context.AccountUrl)
                        .AppendPathSegments(segments)
                        .WithClient(Client);

                    if (!IsAnonymous) url.WithOAuthBearerToken(Context.CurrentProfile?.Token.Access_Token);

                    result = await url
                        .PostJsonAsync(data)
                        .ReceiveJson<T>();

                    // Return or break.
                    break;
                }
                catch (Exception e)
                {
                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    // Determine whether to retry the operation, as well as how
                    // long to wait, based on the retry strategy.
                    if (currentRetry > Constants.RestRetryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential TimeSpan.FromSeconds(Constants.RestRetryDelayInSeconds) here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(TimeSpan.FromSeconds(Constants.RestRetryDelayInSeconds));
            }

            return result;
        }

        public async Task<int> DeleteAccountJsonAsync(object[] segments, object queryparameters = null)
        {
            int result = 0;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await CheckForExpiredToken();

                    // Call external service.
                    result = await new Url(Context.AccountUrl)
                        .AppendPathSegments(segments)
                        .SetQueryParams(queryparameters)
                        .WithClient(Client)
                        .WithOAuthBearerToken(Context.CurrentProfile?.Token.Access_Token)
                        .DeleteAsync()
                        .ReceiveJson<int>();

                    // Return or break.
                    break;
                }
                catch (Exception e)
                {
                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    // Determine whether to retry the operation, as well as how
                    // long to wait, based on the retry strategy.
                    if (currentRetry > Constants.RestRetryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential TimeSpan.FromSeconds(Constants.RestRetryDelayInSeconds) here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(TimeSpan.FromSeconds(Constants.RestRetryDelayInSeconds));
            }

            return result;
        }

        public Task<bool> CheckIfEmailIsAvailableAsync(string email) =>
            GetAccountJsonAsync<bool>(new object[] { ControllerPaths.Check, ControllerPaths.Email, email }, IsAnonymous: true);

        public Task<bool> CheckIfLicenseIsAvailableAsync(string name) =>
            GetAccountJsonAsync<bool>(new object[] { ControllerPaths.Check, ControllerPaths.License, name }, IsAnonymous: true);

        public Task<bool> ForgotPasswordExternal(string email) =>
            GetAccountJsonAsync<bool>(new object[] { ControllerPaths.ForgotPassword, email }, IsAnonymous: true);

        public Task<bool> RegisterExternal(RegistrationData e) =>
            PostAccountJsonAsync<bool>(new object[] { ControllerPaths.RegisterExternal }, e, IsAnonymous: true);

        public Task<bool> AddUserAsync(UserAdd e) =>
            PostAccountJsonAsync<bool>(new object[] { ControllerPaths.Users }, e);

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
