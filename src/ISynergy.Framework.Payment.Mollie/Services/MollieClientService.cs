using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Payment.Mollie.Abstractions.Services;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Converters;
using ISynergy.Framework.Payment.Mollie.Exceptions;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ISynergy.Framework.Payment.Mollie.Services
{
    /// <summary>
    /// Class MollieClientService.
    /// Implements the <see cref="IMollieClientService" />
    /// </summary>
    /// <seealso cref="IMollieClientService" />
    public class MollieClientService : IMollieClientService
    {
        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient _httpClient;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// The json converter service
        /// </summary>
        private readonly JsonConverterService _jsonConverterService;
        /// <summary>
        /// The validator service
        /// </summary>
        private readonly IValidatorService _validatorService;
        /// <summary>
        /// The mollie API options
        /// </summary>
        private readonly MollieApiOptions _mollieApiOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MollieClientService" /> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="validatorService">The validator service.</param>
        public MollieClientService(IHttpClientFactory httpClientFactory, IOptions<MollieApiOptions> options, ILogger<MollieClientService> logger, IValidatorService validatorService = null)
        {
            Argument.IsNotNull(nameof(options), options);
            Argument.IsNotNullOrEmpty(nameof(options.Value.ApiKey), options.Value?.ApiKey);

            _jsonConverterService = new JsonConverterService();
            _httpClient = httpClientFactory.CreateClient();
            _mollieApiOptions = options.Value;
            _logger = logger;
            _validatorService = validatorService ?? new ValidatorService();
        }

        /// <summary>
        /// send HTTP request as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="data">The data.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public async Task<T> SendHttpRequestAsync<T>(HttpMethod httpMethod, string relativeUri, object data = null)
        {
            var httpRequest = CreateHttpRequest(httpMethod, relativeUri);

            if (data != null)
            {
                var jsonData = _jsonConverterService.Serialize(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                httpRequest.Content = content;
            }

            var response = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);
            return await ProcessHttpResponseMessageAsync<T>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the list asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="otherParameters">The other parameters.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> GetListAsync<T>(string relativeUri, string from, int? limit, IDictionary<string, string> otherParameters = null)
        {
            var url = relativeUri + StringConverters.BuildListQueryString(from, limit, otherParameters);
            return SendHttpRequestAsync<T>(HttpMethod.Get, url);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">The relative URI.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> GetAsync<T>(string relativeUri) =>
            SendHttpRequestAsync<T>(HttpMethod.Get, relativeUri);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlObject">The URL object.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> GetAsync<T>(UrlObjectLink<T> urlObject)
        {
            _validatorService.ValidateUrlLink(urlObject);
            return GetAsync<T>(urlObject.Href);
        }

        /// <summary>
        /// Posts the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="data">The data.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> PostAsync<T>(string relativeUri, object data) =>
            SendHttpRequestAsync<T>(HttpMethod.Post, relativeUri, data);

        /// <summary>
        /// Patches the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="data">The data.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public Task<T> PatchAsync<T>(string relativeUri, object data) =>
            SendHttpRequestAsync<T>(new HttpMethod("PATCH"), relativeUri, data);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="data">The data.</param>
        /// <returns>Task.</returns>
        public Task DeleteAsync(string relativeUri, object data = null) =>
            SendHttpRequestAsync<object>(HttpMethod.Delete, relativeUri, data);

        /// <summary>
        /// process HTTP response message as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <returns>T.</returns>
        /// <exception cref="MollieApiException"></exception>
        /// <exception cref="HttpRequestException">Unknown http exception occured with status code: {(int)response.StatusCode}.</exception>
        /// <exception cref="MollieApiException"></exception>
        private async Task<T> ProcessHttpResponseMessageAsync<T>(HttpResponseMessage response)
        {
            var resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return _jsonConverterService.Deserialize<T>(resultContent);
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.UnsupportedMediaType:
                case HttpStatusCode.Gone:
                case (HttpStatusCode)422: // Unprocessable entity
                    throw new MollieApiException(resultContent);
                default:
                    throw new HttpRequestException(
                        $"Unknown http exception occured with status code: {(int)response.StatusCode}.");
            }
        }

        /// <summary>
        /// Creates the HTTP request.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="content">The content.</param>
        /// <returns>HttpRequestMessage.</returns>
        private HttpRequestMessage CreateHttpRequest(HttpMethod method, string relativeUri, HttpContent content = null)
        {
            var httpRequest = new HttpRequestMessage(method, new Uri(new Uri(Constants.ApiEndpoint), relativeUri));
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _mollieApiOptions.ApiKey);
            httpRequest.Content = content;

            return httpRequest;
        }
    }
}
