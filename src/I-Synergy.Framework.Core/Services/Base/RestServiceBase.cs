using Flurl;
using Flurl.Http;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Exceptions;
using ISynergy.Models.General;
using System;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using ISynergy.Events;

namespace ISynergy.Services
{
    public abstract partial class RestServiceBase
    {
        protected IContext _context;
        protected IFlurlClient _client;

        protected int retryCount = 3;
        protected readonly TimeSpan delay = TimeSpan.FromSeconds(5);

        public IFlurlClient RestClient
        {
            get
            {
                if (_client == null)
                {
                    _client = new FlurlClient();
                }

                return _client;
            }
        }

        public RestServiceBase(IContext context)
        {
            _client = new FlurlClient();
            _context = context;
        }

        protected async Task<bool> IsTransient(Exception e)
        {
            bool result = false;

            // Determine if the exception is transient.
            // In some cases this is as simple as checking the exception type, in other
            // cases it might be necessary to inspect other properties of the exception.
            if (e is FlurlHttpException)
            {

                if (e is FlurlHttpException ex)
                {
                    //Check if access-token is expired
                    if (ex.Call.HttpStatus == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await AuthenticateWithRefreshTokenAsync(_context.CurrentProfile?.Token.refresh_token);

                        if (_context.CurrentProfile?.Token == null)
                        {
                            result = false;

                            Messenger.Default.Send(new LoginMessage());
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        protected async Task CheckForExpiredToken()
        {
            if (DateTime.Now.CompareTo(_context.CurrentProfile?.TokenExpiration) >= 0 && _context.CurrentProfile?.Token != null)
            {
                await AuthenticateWithRefreshTokenAsync(_context.CurrentProfile?.Token.refresh_token);
            }
        }

        protected async Task<T> GetJsonAsync<T>(object[] segments, object queryparameters = null, CancellationToken cancellationToken = default)
        {
            T result = default;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await CheckForExpiredToken();

                    // Call external service.
                    result = await new Url(_context.ApiUrl)
                        .AppendPathSegments(segments)
                        .SetQueryParams(queryparameters)
                        .WithClient(RestClient)
                        .WithOAuthBearerToken(_context.CurrentProfile?.Token.access_token)
                        .GetJsonAsync<T>(cancellationToken);

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
                    if (currentRetry > this.retryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential delay here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(delay);
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
                    var url = new Url(_context.AccountUrl)
                        .AppendPathSegments(segments)
                        .SetQueryParams(queryparameters)
                        .WithClient(RestClient);

                    if (!IsAnonymous) url.WithOAuthBearerToken(_context.CurrentProfile?.Token.access_token);

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
                    if (currentRetry > this.retryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential delay here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(delay);
            }

            return result;
        }

        protected async Task<String> GetStringAsync(object[] segments, object queryparameters = null)
        {
            string result = string.Empty;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await CheckForExpiredToken();

                    // Call external service.
                    result = await new Url(_context.ApiUrl)
                        .AppendPathSegments(segments)
                        .SetQueryParams(queryparameters)
                        .WithClient(RestClient)
                        .WithOAuthBearerToken(_context.CurrentProfile?.Token.access_token)
                        .GetStringAsync();

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
                    if (currentRetry > this.retryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential delay here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(delay);
            }

            return result;
        }

        protected Task<int> PostJsonAsync(object[] segments, object data)
        {
            return PostAsync<int>(_context.ApiUrl, segments, data, false);
        }

        protected Task<bool> PostAccountJsonAsync(object[] segments, object data, bool IsAnonymous = false)
        {
            return PostAsync<bool>(_context.AccountUrl, segments, data, IsAnonymous);
        }

        private async Task<T> PostAsync<T>(string baseUrl, object[] segments, object data, bool IsAnonymous = false)
            where T : struct
        {
            T result = default;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await CheckForExpiredToken();

                    // Call external service.
                    var url = new Url(baseUrl)
                        .AppendPathSegments(segments)
                        .WithClient(RestClient);

                    if (!IsAnonymous) url.WithOAuthBearerToken(_context.CurrentProfile?.Token.access_token);

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
                    if (currentRetry > this.retryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential delay here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(delay);
            }

            return result;
        }

        protected Task<int> PutJsonAsync(object[] segments, object data)
        {
            return PutAsync<int>(_context.ApiUrl, segments, data);
        }

        protected Task<T> PutAccountJsonAsync<T>(object[] segments, object data)
            where T : struct
        {
            return PutAsync<T>(_context.AccountUrl, segments, data);
        }

        private async Task<T> PutAsync<T>(string baseUrl, object[] segments, object data)
            where T : struct
        {
            T result = default;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await CheckForExpiredToken();

                    // Call external service.
                    result = await new Url(baseUrl)
                        .AppendPathSegments(segments)
                        .WithClient(RestClient)
                        .WithOAuthBearerToken(_context.CurrentProfile?.Token.access_token)
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
                    if (currentRetry > this.retryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential delay here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(delay);
            }

            return result;
        }

        protected Task<int> DeleteJsonAsync(object[] segments, object queryparameters = null)
        {
            return DeleteAsync(_context.ApiUrl, segments, queryparameters);
        }

        protected Task<int> DeleteAccountJsonAsync(object[] segments, object queryparameters = null)
        {
            return DeleteAsync(_context.AccountUrl, segments, queryparameters);
        }

        private async Task<int> DeleteAsync(string baseUrl, object[] segments, object queryparameters = null)
        {
            int result = 0;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await CheckForExpiredToken();

                    // Call external service.
                    result = await new Url(baseUrl)
                        .AppendPathSegments(segments)
                        .SetQueryParams(queryparameters)
                        .WithClient(RestClient)
                        .WithOAuthBearerToken(_context.CurrentProfile?.Token.access_token)
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
                    if (currentRetry > this.retryCount || !await IsTransient(e))
                    {
                        // If this isn't a transient error or we shouldn't retry,
                        // rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential delay here and
                // using a strategy best suited for the operation and fault.
                await Task.Delay(delay);
            }

            return result;
        }

        public async Task AuthenticateWithTokenAsync(string username, string password)
        {
            Argument.IsNotNullOrEmpty(nameof(username), username);
            Argument.IsNotNullOrEmpty(nameof(password), password);

            try
            {
                var token = await new Url(_context.TokenUrl)
                        .WithClient(RestClient)
                        .PostUrlEncodedAsync(new Grant
                        {
                            grant_type = Grant_Types.password,
                            username = username,
                            password = password,
                            client_id = _context.Client_Id,
                            client_secret = _context.Client_Secret,
                            scope = Grant_Scopes.password
                        })
                        .ReceiveJson<Token>();

                if(token != null)
                {
                    _context.CurrentProfile = new Profile
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
                _context.CurrentProfile = new Profile
                {
                    Token = await new Url(_context.TokenUrl)
                        .WithClient(RestClient)
                        .PostUrlEncodedAsync(new Grant
                        {
                            grant_type = Grant_Types.client_credentials,
                            client_id = _context.Client_Id,
                            client_secret = _context.Client_Secret,
                            scope = Grant_Scopes.client_credentials
                        })
                        .ReceiveJson<Token>()
                };
            }
            catch (FlurlHttpException e)
            {
                await HandleFlurlExceptionAsync(e);
            }
        }

        private async Task HandleFlurlExceptionAsync(FlurlHttpException e)
        {
            if (e.Call.HttpStatus == System.Net.HttpStatusCode.BadRequest)
            {
                ApiException api_ex = await e.GetResponseJsonAsync<ApiException>();

                if (api_ex != null)
                {
                    if (api_ex.error == Globals.InvalidGrantError)
                    {
                        await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_ACCOUNT_LOGIN_FAILED"));
                    }
                    else if (api_ex.error == Globals.AuthenticationError || api_ex.error == Globals.UnauthorizedClientError)
                    {
                        await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString(api_ex.error_description));
                    }
                    else
                    {
                        await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_ACCOUNT_OTHER"));
                    }
                }
            }
            else
            {
                await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_ACCOUNT_OTHER"));
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
                _context.CurrentProfile = new Profile
                {
                    Token = await new Url(_context.TokenUrl)
                        .WithClient(RestClient)
                        .PostUrlEncodedAsync(new Grant
                        {
                            grant_type = Grant_Types.refresh_token,
                            refresh_token = refreshtoken,
                            client_id = _context.Client_Id,
                            client_secret = _context.Client_Secret
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
                        await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                                ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_ACCOUNT_LOGIN_FAILED"));
                    }
                }

                Messenger.Default.Send(new LoginMessage());
            }
        }
    }
}