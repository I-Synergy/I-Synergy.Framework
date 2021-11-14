using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Subscription;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class SubscriptionClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="ISubscriptionClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="ISubscriptionClient" />
    public class SubscriptionClient : MollieClientBase, ISubscriptionClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public SubscriptionClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<SubscriptionClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Gets the subscription list asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;SubscriptionResponse&gt;&gt;.</returns>
        public Task<ListResponse<SubscriptionResponse>> GetSubscriptionListAsync(string customerId, string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<SubscriptionResponse>>($"customers/{customerId}/subscriptions", from, limit);

        /// <summary>
        /// Gets the subscription list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;SubscriptionResponse&gt;&gt;.</returns>
        public Task<ListResponse<SubscriptionResponse>> GetSubscriptionListAsync(UrlObjectLink<ListResponse<SubscriptionResponse>> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Gets the subscription asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;SubscriptionResponse&gt;.</returns>
        public Task<SubscriptionResponse> GetSubscriptionAsync(UrlObjectLink<SubscriptionResponse> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Gets the subscription asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>Task&lt;SubscriptionResponse&gt;.</returns>
        public Task<SubscriptionResponse> GetSubscriptionAsync(string customerId, string subscriptionId) =>
            _clientService.GetAsync<SubscriptionResponse>($"customers/{customerId}/subscriptions/{subscriptionId}");

        /// <summary>
        /// Creates the subscription asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;SubscriptionResponse&gt;.</returns>
        public Task<SubscriptionResponse> CreateSubscriptionAsync(string customerId, SubscriptionRequest request) =>
            _clientService.PostAsync<SubscriptionResponse>($"customers/{customerId}/subscriptions", request);

        /// <summary>
        /// Cancels the subscription asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>Task.</returns>
        public Task CancelSubscriptionAsync(string customerId, string subscriptionId) =>
            _clientService.DeleteAsync($"customers/{customerId}/subscriptions/{subscriptionId}");

        /// <summary>
        /// Updates the subscription asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;SubscriptionResponse&gt;.</returns>
        public Task<SubscriptionResponse> UpdateSubscriptionAsync(string customerId, string subscriptionId, SubscriptionUpdateRequest request) =>
            _clientService.PatchAsync<SubscriptionResponse>($"customers/{customerId}/subscriptions/{subscriptionId}", request);

        /// <summary>
        /// Gets the subscription payment list asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
        public Task<ListResponse<PaymentResponse>> GetSubscriptionPaymentListAsync(string customerId, string subscriptionId, string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<PaymentResponse>>($"customers/{customerId}/subscriptions/{subscriptionId}/payments", from, limit);
    }
}
