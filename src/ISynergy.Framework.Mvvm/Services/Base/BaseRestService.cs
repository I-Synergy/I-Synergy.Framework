using Flurl;
using Flurl.Http;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Options;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Services.Base
{
    /// <summary>
    /// Base rest service.
    /// </summary>
    internal abstract class BaseRestService
    {
        /// <summary>
        /// The client
        /// </summary>
        protected readonly IFlurlClient _client;
        /// <summary>
        /// The context
        /// </summary>
        protected readonly IContext _context;
        /// <summary>
        /// The authentication service
        /// </summary>
        protected readonly IBaseAuthenticationService _authenticationService;
        /// <summary>
        /// The language service
        /// </summary>
        protected readonly ILanguageService _languageService;
        /// <summary>
        /// The exception handler service.
        /// </summary>
        protected readonly IExceptionHandlerService _exceptionHandlerService;
        /// <summary>
        /// The configuration options
        /// </summary>
        protected readonly IConfigurationOptions _configurationOptions;
        /// <summary>
        /// The retry policy
        /// </summary>
        protected readonly AsyncRetryPolicy _retryPolicy;

        /// <summary>
        /// Initializes a new instance of the Rest Api Service.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="client">The client.</param>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="languageService">The language service.</param>
        /// <param name="exceptionHandlerService">The exception handler service.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        protected BaseRestService(
            IContext context,
            IFlurlClient client,
            IBaseAuthenticationService authenticationService,
            ILanguageService languageService,
            IExceptionHandlerService exceptionHandlerService,
            IOptions<IConfigurationOptions> configurationOptions)
        {
            _client = client;
            _context = context;
            _authenticationService = authenticationService;
            _languageService = languageService;
            _exceptionHandlerService = exceptionHandlerService;
            _configurationOptions = configurationOptions.Value;

            _retryPolicy = Policy
                .Handle<Exception>()
                .Or<FlurlHttpException>()
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(
                    GenericConstants.RestRetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, context) =>
                    {
                        if (exception is FlurlHttpException flurlException)
                            _exceptionHandlerService.HandleExceptionAsync(new ApiException(flurlException.Message, nameof(FlurlHttpException)));
                        else
                            _exceptionHandlerService.HandleExceptionAsync(exception);
                    });
        }

        /// <summary>
        /// get json as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="segments">The segments.</param>
        /// <param name="queryparameters">The queryparameters.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="TaskCanceledException"></exception>
        public async Task<T> GetJsonAsync<T>(object[] segments, object queryparameters, CancellationToken cancellationToken = default)
        {
            T result = default;

            await _retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                await _authenticationService.CheckForExpiredToken();

                // Call external service.
                result = await new Url(_configurationOptions.ServiceEndpoint)
                    .AppendPathSegments(segments)
                    .SetQueryParams(queryparameters)
                    .WithClient(_client)
                    .WithOAuthBearerToken(_context.CurrentProfile?.Token.AccessToken)
                    .GetJsonAsync<T>(cancellationToken);
            });

            return result;
        }

        /// <summary>
        /// get string as an asynchronous operation.
        /// </summary>
        /// <param name="segments">The segments.</param>
        /// <param name="queryparameters">The queryparameters.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        /// <exception cref="TaskCanceledException"></exception>
        public async Task<string> GetStringAsync(object[] segments, object queryparameters, CancellationToken cancellationToken = default)
        {
            string result = default;

            await _retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                await _authenticationService.CheckForExpiredToken();

                // Call external service.
                result = await new Url(_configurationOptions.ServiceEndpoint)
                    .AppendPathSegments(segments)
                    .SetQueryParams(queryparameters)
                    .WithClient(_client)
                    .WithOAuthBearerToken(_context.CurrentProfile?.Token.AccessToken)
                    .GetJsonAsync<string>(cancellationToken);
            });

            return result;
        }

        /// <summary>
        /// Posts the json asynchronous.
        /// </summary>
        /// <param name="segments">The segments.</param>
        /// <param name="data">The data.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> PostJsonAsync(object[] segments, object data, CancellationToken cancellationToken = default) =>
            PostAsync<int>(_configurationOptions.ServiceEndpoint, segments, data, false, cancellationToken);

        /// <summary>
        /// post as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="segments">The segments.</param>
        /// <param name="data">The data.</param>
        /// <param name="IsAnonymous">if set to <c>true</c> [is anonymous].</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="TaskCanceledException"></exception>
        public async Task<T> PostAsync<T>(string baseUrl, object[] segments, object data, bool IsAnonymous = false, CancellationToken cancellationToken = default)
        {
            T result = default;

            await _retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                await _authenticationService.CheckForExpiredToken();

                // Call external service.
                var url = new Url(baseUrl)
                    .AppendPathSegments(segments)
                    .WithClient(_client);

                if (!IsAnonymous) url.WithOAuthBearerToken(_context.CurrentProfile?.Token.AccessToken);

                result = await url
                    .PostJsonAsync(data, cancellationToken)
                    .ReceiveJson<T>();
            });

            return result;
        }

        /// <summary>
        /// post as an asynchronous operation.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="segments">The segments.</param>
        /// <param name="data">The data.</param>
        /// <param name="IsAnonymous">if set to <c>true</c> [is anonymous].</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="TaskCanceledException"></exception>
        public async Task<byte[]> PostGetBytesAsync(string baseUrl, object[] segments, object data, bool IsAnonymous = false, CancellationToken cancellationToken = default)
        {
            byte[] result = default;

            await _retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                await _authenticationService.CheckForExpiredToken();

                // Call external service.
                var url = new Url(baseUrl)
                    .AppendPathSegments(segments)
                    .WithClient(_client);

                if (!IsAnonymous) url.WithOAuthBearerToken(_context.CurrentProfile?.Token.AccessToken);

                result = await url
                    .PostJsonAsync(data, cancellationToken)
                    .ReceiveBytes();
            });

            return result;
        }

        /// <summary>
        /// Puts the json asynchronous.
        /// </summary>
        /// <param name="segments">The segments.</param>
        /// <param name="data">The data.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> PutJsonAsync(object[] segments, object data, CancellationToken cancellationToken = default) =>
            PutAsync<int>(_configurationOptions.ServiceEndpoint, segments, data, cancellationToken);

        /// <summary>
        /// put as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="segments">The segments.</param>
        /// <param name="data">The data.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="TaskCanceledException"></exception>
        public async Task<T> PutAsync<T>(string baseUrl, object[] segments, object data, CancellationToken cancellationToken = default)
        {
            T result = default;

            await _retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                await _authenticationService.CheckForExpiredToken();

                // Call external service.
                result = await new Url(baseUrl)
                    .AppendPathSegments(segments)
                    .WithClient(_client)
                    .WithOAuthBearerToken(_context.CurrentProfile?.Token.AccessToken)
                    .PutJsonAsync(data, cancellationToken)
                    .ReceiveJson<T>();
            });

            return result;
        }

        /// <summary>
        /// Deletes the json asynchronous.
        /// </summary>
        /// <param name="segments">The segments.</param>
        /// <param name="queryparameters">The queryparameters.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> DeleteJsonAsync(object[] segments, object queryparameters = null, CancellationToken cancellationToken = default) =>
            DeleteAsync(_configurationOptions.ServiceEndpoint, segments, queryparameters, cancellationToken);

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="segments">The segments.</param>
        /// <param name="queryparameters">The queryparameters.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="TaskCanceledException"></exception>
        public async Task<int> DeleteAsync(string baseUrl, object[] segments, object queryparameters = null, CancellationToken cancellationToken = default)
        {
            int result = default;

            await _retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                await _authenticationService.CheckForExpiredToken();

                // Call external service.
                result = await new Url(baseUrl)
                    .AppendPathSegments(segments)
                    .SetQueryParams(queryparameters)
                    .WithClient(_client)
                    .WithOAuthBearerToken(_context.CurrentProfile?.Token.AccessToken)
                    .DeleteAsync(cancellationToken)
                    .ReceiveJson<int>();
            });

            return result;
        }

        /// <summary>
        /// Downloads the asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>System.Threading.Tasks.Task&lt;byte[]&gt;.</returns>
        public Task<byte[]> DownloadAsync(Uri url, CancellationToken cancellationToken = default) =>
            new Url(url).GetBytesAsync(cancellationToken);
    }
}
