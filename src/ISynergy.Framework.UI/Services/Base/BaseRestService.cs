using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ISynergy.Framework.UI.Services.Base;

/// <summary>
/// Base rest service.
/// </summary>
public abstract class BaseRestService
{
    private readonly int _retryCount;
    
    /// <summary>
    /// Json serializer options.
    /// </summary>
    protected readonly JsonSerializerOptions _jsonSerializerOptions;
    /// <summary>
    /// The context
    /// </summary>
    protected readonly IContext _context;
    /// <summary>
    /// Authentication service.
    /// </summary>
    protected readonly IAuthenticationService _authenticationService;
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
    protected readonly ConfigurationOptions _configurationOptions;

    /// <summary>
    /// Initializes a new instance of the Rest Api Service.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="authenticationService"></param>
    /// <param name="languageService">The language service.</param>
    /// <param name="exceptionHandlerService">The exception handler service.</param>
    /// <param name="jsonSerializerOptions"></param>
    /// <param name="configurationOptions">The configuration options.</param>
    /// <param name="retryCount"></param>
    protected BaseRestService(
        IContext context,
        IAuthenticationService authenticationService,
        ILanguageService languageService,
        IExceptionHandlerService exceptionHandlerService,
        JsonSerializerOptions jsonSerializerOptions,
        IOptions<ConfigurationOptions> configurationOptions,
        int retryCount = 3)
    {
        _context = context;
        _authenticationService = authenticationService;
        _languageService = languageService;
        _exceptionHandlerService = exceptionHandlerService;
        _configurationOptions = configurationOptions.Value;
        _jsonSerializerOptions = jsonSerializerOptions;
        _retryCount = retryCount;
    }

    /// <summary>
    /// get json as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url">The base URL.</param>
    /// <param name="anonymous">if set to <c>true</c> [is anonymous].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    /// <exception cref="TaskCanceledException"></exception>
    public virtual async Task<T> GetAsync<T>(string url, bool anonymous = false, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        if (!_context.IsAuthenticated && _context.Profile is not null)
            await _authenticationService.AuthenticateWithRefreshTokenAsync(_context.Profile.Token.RefreshToken, cancellationToken);

        if (anonymous || _context.IsAuthenticated)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(nameof(Grant.client_id), _configurationOptions.ClientId);
                client.DefaultRequestHeaders.Add(nameof(Grant.client_secret), _configurationOptions.ClientSecret);

                if (!anonymous)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _context.Profile.Token.AccessToken);

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return default(T);

                    return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions, cancellationToken);
                }
                else
                {
                    return default(T);
                }

            }
        }

        throw new UnauthorizedAccessException();
    }

    /// <summary>
    /// get string as an asynchronous operation.
    /// </summary>
    /// <param name="url">The base URL.</param>
    /// <param name="anonymous">if set to <c>true</c> [is anonymous].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    /// <exception cref="TaskCanceledException"></exception>
    public virtual async Task<string> GetStringAsync(string url, bool anonymous = false, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        if (!_context.IsAuthenticated && _context.Profile is not null)
            await _authenticationService.AuthenticateWithRefreshTokenAsync(_context.Profile.Token.RefreshToken, cancellationToken);

        if (anonymous || _context.IsAuthenticated)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(nameof(Grant.client_id), _configurationOptions.ClientId);
                client.DefaultRequestHeaders.Add(nameof(Grant.client_secret), _configurationOptions.ClientSecret);

                if (!anonymous)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _context.Profile.Token.AccessToken);

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return string.Empty;

                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return default;
                }
            }
        }

        throw new UnauthorizedAccessException();
    }

    /// <summary>
    /// Downloads as an asynchronous operation.
    /// </summary>
    /// <param name="url">The base URL.</param>
    /// <param name="anonymous">if set to <c>true</c> [is anonymous].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    /// <exception cref="TaskCanceledException"></exception>
    public virtual async Task<byte[]> DownloadAsync(string url, bool anonymous = false, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        if (!_context.IsAuthenticated && _context.Profile is not null)
            await _authenticationService.AuthenticateWithRefreshTokenAsync(_context.Profile.Token.RefreshToken, cancellationToken);

        if (anonymous || _context.IsAuthenticated)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(nameof(Grant.client_id), _configurationOptions.ClientId);
                client.DefaultRequestHeaders.Add(nameof(Grant.client_secret), _configurationOptions.ClientSecret);

                if (!anonymous)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _context.Profile.Token.AccessToken);

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return null;

                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    return null;
                }
            }
        }

        throw new UnauthorizedAccessException();
    }

    /// <summary>
    /// post as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url">The base URL.</param>
    /// <param name="data">The data.</param>
    /// <param name="anonymous">if set to <c>true</c> [is anonymous].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    /// <exception cref="TaskCanceledException"></exception>
    public virtual async Task<T> PostAsync<T>(string url, object data, bool anonymous = false, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        if (!_context.IsAuthenticated && _context.Profile is not null)
            await _authenticationService.AuthenticateWithRefreshTokenAsync(_context.Profile.Token.RefreshToken, cancellationToken);

        if (anonymous || _context.IsAuthenticated)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(nameof(Grant.client_id), _configurationOptions.ClientId);
                client.DefaultRequestHeaders.Add(nameof(Grant.client_secret), _configurationOptions.ClientSecret);

                if (!anonymous)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _context.Profile.Token.AccessToken);

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(data, data.GetType(), new MediaTypeHeaderValue("application/json"), _jsonSerializerOptions)
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return default(T);

                    return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions, cancellationToken);
                }
                else
                {
                    return default(T);
                }
            }
        }

        throw new UnauthorizedAccessException();
    }

    /// <summary>
    /// post as an asynchronous operation.
    /// </summary>
    /// <param name="url">The base URL.</param>
    /// <param name="data">The data.</param>
    /// <param name="anonymous">if set to <c>true</c> [is anonymous].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    /// <exception cref="TaskCanceledException"></exception>
    public virtual async Task<byte[]> PostBytesAsync(string url, object data, bool anonymous = false, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        if (!_context.IsAuthenticated && _context.Profile is not null)
            await _authenticationService.AuthenticateWithRefreshTokenAsync(_context.Profile.Token.RefreshToken, cancellationToken);

        if (anonymous || _context.IsAuthenticated)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(nameof(Grant.client_id), _configurationOptions.ClientId);
                client.DefaultRequestHeaders.Add(nameof(Grant.client_secret), _configurationOptions.ClientSecret);

                if (!anonymous)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _context.Profile.Token.AccessToken);

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(data, data.GetType(), new MediaTypeHeaderValue("application/json"), _jsonSerializerOptions)
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return null;

                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    return null;
                }
            }
        }

        throw new UnauthorizedAccessException();
    }

    /// <summary>
    /// put as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url">The base URL.</param>
    /// <param name="data">The data.</param>
    /// <param name="anonymous">if set to <c>true</c> [is anonymous].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;T&gt;.</returns>
    /// <exception cref="TaskCanceledException"></exception>
    public virtual async Task<T> PutAsync<T>(string url, object data, bool anonymous = false, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        if (!_context.IsAuthenticated && _context.Profile is not null)
            await _authenticationService.AuthenticateWithRefreshTokenAsync(_context.Profile.Token.RefreshToken, cancellationToken);

        if (anonymous || _context.IsAuthenticated)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(nameof(Grant.client_id), _configurationOptions.ClientId);
                client.DefaultRequestHeaders.Add(nameof(Grant.client_secret), _configurationOptions.ClientSecret);

                if (!anonymous)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _context.Profile.Token.AccessToken);

                var request = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = JsonContent.Create(data, data.GetType(), new MediaTypeHeaderValue("application/json"), _jsonSerializerOptions)
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return default(T);

                    return await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions, cancellationToken);
                }
                else
                {
                    return default(T);
                }
            }
        }

        throw new UnauthorizedAccessException();
    }

    /// <summary>
    /// delete as an asynchronous operation.
    /// </summary>
    /// <param name="url">The base URL.</param>
    /// <param name="anonymous">if set to <c>true</c> [is anonymous].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="TaskCanceledException"></exception>
    public virtual async Task<int> DeleteAsync(string url, bool anonymous = false, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        if (!_context.IsAuthenticated && _context.Profile is not null)
            await _authenticationService.AuthenticateWithRefreshTokenAsync(_context.Profile.Token.RefreshToken, cancellationToken);

        if (anonymous || _context.IsAuthenticated)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(nameof(Grant.client_id), _configurationOptions.ClientId);
                client.DefaultRequestHeaders.Add(nameof(Grant.client_secret), _configurationOptions.ClientSecret);

                if (!anonymous)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _context.Profile.Token.AccessToken);

                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return 0;

                    return await response.Content.ReadFromJsonAsync<int>(_jsonSerializerOptions, cancellationToken);
                }
                else
                {
                    return 0;
                }
            }
        }

        throw new UnauthorizedAccessException();
    }
}
