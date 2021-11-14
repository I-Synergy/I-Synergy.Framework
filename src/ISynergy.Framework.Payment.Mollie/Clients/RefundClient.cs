using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Refund;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class RefundClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IRefundClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IRefundClient" />
    public class RefundClient : MollieClientBase, IRefundClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefundClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public RefundClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<RefundClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Creates the refund asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="refundRequest">The refund request.</param>
        /// <returns>Task&lt;RefundResponse&gt;.</returns>
        public Task<RefundResponse> CreateRefundAsync(string paymentId, RefundRequest refundRequest)
        {
            if (refundRequest.Testmode.HasValue)
            {
                ValidateApiKeyIsOauthAccesstoken();
            }

            return _clientService.PostAsync<RefundResponse>($"payments/{paymentId}/refunds", refundRequest);
        }

        /// <summary>
        /// Gets the refund list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;RefundResponse&gt;&gt;.</returns>
        public Task<ListResponse<RefundResponse>> GetRefundListAsync(string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<RefundResponse>>($"refunds", from, limit);

        /// <summary>
        /// Gets the refund list asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;RefundResponse&gt;&gt;.</returns>
        public Task<ListResponse<RefundResponse>> GetRefundListAsync(string paymentId, string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<RefundResponse>>($"payments/{paymentId}/refunds", from, limit);

        /// <summary>
        /// Gets the refund asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;RefundResponse&gt;.</returns>
        public Task<RefundResponse> GetRefundAsync(UrlObjectLink<RefundResponse> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Gets the refund asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="refundId">The refund identifier.</param>
        /// <returns>Task&lt;RefundResponse&gt;.</returns>
        public Task<RefundResponse> GetRefundAsync(string paymentId, string refundId) =>
            _clientService.GetAsync<RefundResponse>($"payments/{paymentId}/refunds/{refundId}");

        /// <summary>
        /// Cancels the refund asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="refundId">The refund identifier.</param>
        /// <returns>Task.</returns>
        public Task CancelRefundAsync(string paymentId, string refundId) =>
            _clientService.DeleteAsync($"payments/{paymentId}/refunds/{refundId}");
    }
}
