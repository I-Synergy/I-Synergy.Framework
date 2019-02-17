using Flurl;
using Flurl.Http;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public abstract class BaseRestService : IBaseRestService
    {
        public IContext Context { get; }
        public IFlurlClient Client { get; }
        public IAuthenticationService AuthenticationService { get; }
        
        public BaseRestService(
            IContext context,
            IFlurlClient client,
            IAuthenticationService authenticationService)
        {
            Client = client;
            Context = context;
            AuthenticationService = authenticationService;
        }
        
        public async Task<T> GetJsonAsync<T>(object[] segments, object queryparameters, CancellationToken cancellationToken)
        {
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();

                    await AuthenticationService.CheckForExpiredToken();

                    // Call external service.
                    return await new Url(Context.ApiUrl)
                        .AppendPathSegments(segments)
                        .SetQueryParams(queryparameters)
                        .WithClient(Client)
                        .WithOAuthBearerToken(Context.CurrentProfile?.Token.access_token)
                        .GetJsonAsync<T>(cancellationToken);
                }
                catch (Exception f) 
                    when (f.InnerException is TaskCanceledException | f.InnerException is OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    // Determine whether to retry the operation, as well as how
                    // long to wait, based on the retry strategy.
                    if (currentRetry > Constants.RestRetryCount || !await AuthenticationService.IsTransient(e))
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
        }

        public async Task<string> GetStringAsync(object[] segments, object queryparameters, CancellationToken cancellationToken)
        {
            string result = string.Empty;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();

                    await AuthenticationService.CheckForExpiredToken();

                    // Call external service.
                    result = await new Url(Context.ApiUrl)
                        .AppendPathSegments(segments)
                        .SetQueryParams(queryparameters)
                        .WithClient(Client)
                        .WithOAuthBearerToken(Context.CurrentProfile?.Token.access_token)
                        .GetStringAsync(cancellationToken);

                    // Return or break.
                    break;
                }
                catch (Exception f) 
                    when (f.InnerException is TaskCanceledException || f.InnerException is OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    // Determine whether to retry the operation, as well as how
                    // long to wait, based on the retry strategy.
                    if (currentRetry > Constants.RestRetryCount || !await AuthenticationService.IsTransient(e))
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

        public Task<int> PostJsonAsync(object[] segments, object data) =>
            PostAsync<int>(Context.ApiUrl, segments, data, false);

        public async Task<T> PostAsync<T>(string baseUrl, object[] segments, object data, bool IsAnonymous = false)
        {
            T result = default;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await AuthenticationService.CheckForExpiredToken();

                    // Call external service.
                    var url = new Url(baseUrl)
                        .AppendPathSegments(segments)
                        .WithClient(Client);

                    if (!IsAnonymous) url.WithOAuthBearerToken(Context.CurrentProfile?.Token.access_token);

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
                    if (currentRetry > Constants.RestRetryCount || !await AuthenticationService.IsTransient(e))
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

        public Task<int> PutJsonAsync(object[] segments, object data) =>
            PutAsync<int>(Context.ApiUrl, segments, data);

        public async Task<T> PutAsync<T>(string baseUrl, object[] segments, object data)
        {
            T result = default;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await AuthenticationService.CheckForExpiredToken();

                    // Call external service.
                    result = await new Url(baseUrl)
                        .AppendPathSegments(segments)
                        .WithClient(Client)
                        .WithOAuthBearerToken(Context.CurrentProfile?.Token.access_token)
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
                    if (currentRetry > Constants.RestRetryCount || !await AuthenticationService.IsTransient(e))
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

        public Task<int> DeleteJsonAsync(object[] segments, object queryparameters = null) =>
            DeleteAsync(Context.ApiUrl, segments, queryparameters);

        public async Task<int> DeleteAsync(string baseUrl, object[] segments, object queryparameters = null)
        {
            int result = 0;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    await AuthenticationService.CheckForExpiredToken();

                    // Call external service.
                    result = await new Url(baseUrl)
                        .AppendPathSegments(segments)
                        .SetQueryParams(queryparameters)
                        .WithClient(Client)
                        .WithOAuthBearerToken(Context.CurrentProfile?.Token.access_token)
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
                    if (currentRetry > Constants.RestRetryCount || !await AuthenticationService.IsTransient(e))
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
    }
}