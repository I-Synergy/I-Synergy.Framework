using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Order;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Refund;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class OrderClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IOrderClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IOrderClient" />
    public class OrderClient : MollieClientBase, IOrderClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public OrderClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<OrderClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Creates the order asynchronous.
        /// </summary>
        /// <param name="orderRequest">The order request.</param>
        /// <returns>Task&lt;OrderResponse&gt;.</returns>
        public Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest) =>
            _clientService.PostAsync<OrderResponse>("orders", orderRequest);

        /// <summary>
        /// Gets the order asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>Task&lt;OrderResponse&gt;.</returns>
        public Task<OrderResponse> GetOrderAsync(string orderId) =>
            _clientService.GetAsync<OrderResponse>($"orders/{orderId}");

        /// <summary>
        /// Updates the order asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderUpdateRequest">The order update request.</param>
        /// <returns>Task&lt;OrderResponse&gt;.</returns>
        public Task<OrderResponse> UpdateOrderAsync(string orderId, OrderUpdateRequest orderUpdateRequest) =>
            _clientService.PatchAsync<OrderResponse>($"orders/{orderId}", orderUpdateRequest);

        /// <summary>
        /// Updates the order lines asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLineId">The order line identifier.</param>
        /// <param name="orderLineUpdateRequest">The order line update request.</param>
        /// <returns>Task&lt;OrderResponse&gt;.</returns>
        public Task<OrderResponse> UpdateOrderLinesAsync(string orderId, string orderLineId, OrderLineUpdateRequest orderLineUpdateRequest) =>
            _clientService.PatchAsync<OrderResponse>($"orders/{orderId}/lines/{orderLineId}", orderLineUpdateRequest);

        /// <summary>
        /// Cancels the order asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>Task.</returns>
        public Task CancelOrderAsync(string orderId) =>
            _clientService.DeleteAsync($"orders/{orderId}");

        /// <summary>
        /// Gets the order list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;OrderResponse&gt;&gt;.</returns>
        public Task<ListResponse<OrderResponse>> GetOrderListAsync(string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<OrderResponse>>($"orders", from, limit);

        /// <summary>
        /// Gets the order list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;OrderResponse&gt;&gt;.</returns>
        public Task<ListResponse<OrderResponse>> GetOrderListAsync(UrlObjectLink<ListResponse<OrderResponse>> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Cancels the order lines asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="cancelationRequest">The cancelation request.</param>
        /// <returns>Task.</returns>
        public Task CancelOrderLinesAsync(string orderId, OrderLineCancellationRequest cancelationRequest) =>
            _clientService.DeleteAsync($"orders/{orderId}/lines", cancelationRequest);

        /// <summary>
        /// Creates the order payment asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="createOrderPaymentRequest">The create order payment request.</param>
        /// <returns>Task&lt;PaymentResponse&gt;.</returns>
        public Task<PaymentResponse> CreateOrderPaymentAsync(string orderId, OrderPaymentRequest createOrderPaymentRequest) =>
            _clientService.PostAsync<PaymentResponse>($"orders/{orderId}/payments", createOrderPaymentRequest);

        /// <summary>
        /// Creates the order refund asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="createOrderRefundRequest">The create order refund request.</param>
        /// <returns>Task.</returns>
        public Task CreateOrderRefundAsync(string orderId, OrderRefundRequest createOrderRefundRequest) =>
            _clientService.DeleteAsync($"orders/{orderId}/refunds", createOrderRefundRequest);

        /// <summary>
        /// Gets the order refund list asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;RefundResponse&gt;&gt;.</returns>
        public Task<ListResponse<RefundResponse>> GetOrderRefundListAsync(string orderId, string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<RefundResponse>>($"orders/{orderId}/refunds", from, limit);
    }
}
