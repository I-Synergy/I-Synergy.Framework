using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Subscription;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface ISubscriptionClient
    /// </summary>
    public interface ISubscriptionClient
    {
        /// <summary>
        /// Cancels the subscription asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>Task.</returns>
        Task CancelSubscriptionAsync(string customerId, string subscriptionId);
        /// <summary>
        /// Creates the subscription asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;SubscriptionResponse&gt;.</returns>
        Task<SubscriptionResponse> CreateSubscriptionAsync(string customerId, SubscriptionRequest request);
        /// <summary>
        /// Gets the subscription asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>Task&lt;SubscriptionResponse&gt;.</returns>
        Task<SubscriptionResponse> GetSubscriptionAsync(string customerId, string subscriptionId);
        /// <summary>
        /// Gets the subscription list asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;SubscriptionResponse&gt;&gt;.</returns>
        Task<ListResponse<SubscriptionResponse>> GetSubscriptionListAsync(string customerId, string from = null, int? limit = null);
        /// <summary>
        /// Gets the subscription list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;SubscriptionResponse&gt;&gt;.</returns>
        Task<ListResponse<SubscriptionResponse>> GetSubscriptionListAsync(UrlObjectLink<ListResponse<SubscriptionResponse>> url);
        /// <summary>
        /// Gets the subscription asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;SubscriptionResponse&gt;.</returns>
        Task<SubscriptionResponse> GetSubscriptionAsync(UrlObjectLink<SubscriptionResponse> url);
        /// <summary>
        /// Updates the subscription asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;SubscriptionResponse&gt;.</returns>
        Task<SubscriptionResponse> UpdateSubscriptionAsync(string customerId, string subscriptionId, SubscriptionUpdateRequest request);
        /// <summary>
        /// Gets the subscription payment list asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
        Task<ListResponse<PaymentResponse>> GetSubscriptionPaymentListAsync(string customerId, string subscriptionId, string from = null, int? limit = null);
    }
}
