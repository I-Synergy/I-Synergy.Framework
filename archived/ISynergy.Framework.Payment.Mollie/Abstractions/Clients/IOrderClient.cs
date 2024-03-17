using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Order;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Refund;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IOrderClient
    /// </summary>
    public interface IOrderClient
    {
        /// <summary>
        /// Creates the order asynchronous.
        /// </summary>
        /// <param name="orderRequest">The order request.</param>
        /// <returns>Task&lt;OrderResponse&gt;.</returns>
        Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest);
        /// <summary>
        /// Gets the order asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>Task&lt;OrderResponse&gt;.</returns>
        Task<OrderResponse> GetOrderAsync(string orderId);
        /// <summary>
        /// Updates the order asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderUpdateRequest">The order update request.</param>
        /// <returns>Task&lt;OrderResponse&gt;.</returns>
        Task<OrderResponse> UpdateOrderAsync(string orderId, OrderUpdateRequest orderUpdateRequest);
        /// <summary>
        /// Updates the order lines asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLineId">The order line identifier.</param>
        /// <param name="orderLineUpdateRequest">The order line update request.</param>
        /// <returns>Task&lt;OrderResponse&gt;.</returns>
        Task<OrderResponse> UpdateOrderLinesAsync(string orderId, string orderLineId, OrderLineUpdateRequest orderLineUpdateRequest);
        /// <summary>
        /// Cancels the order asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>Task.</returns>
        Task CancelOrderAsync(string orderId);
        /// <summary>
        /// Gets the order list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;OrderResponse&gt;&gt;.</returns>
        Task<ListResponse<OrderResponse>> GetOrderListAsync(string from = null, int? limit = null);
        /// <summary>
        /// Gets the order list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;OrderResponse&gt;&gt;.</returns>
        Task<ListResponse<OrderResponse>> GetOrderListAsync(UrlObjectLink<ListResponse<OrderResponse>> url);
        /// <summary>
        /// Cancels the order lines asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="cancelationRequest">The cancelation request.</param>
        /// <returns>Task.</returns>
        Task CancelOrderLinesAsync(string orderId, OrderLineCancellationRequest cancelationRequest);
        /// <summary>
        /// Creates the order payment asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="createOrderPaymentRequest">The create order payment request.</param>
        /// <returns>Task&lt;PaymentResponse&gt;.</returns>
        Task<PaymentResponse> CreateOrderPaymentAsync(string orderId, OrderPaymentRequest createOrderPaymentRequest);
        /// <summary>
        /// Creates the order refund asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="createOrderRefundRequest">The create order refund request.</param>
        /// <returns>Task.</returns>
        Task CreateOrderRefundAsync(string orderId, OrderRefundRequest createOrderRefundRequest);
        /// <summary>
        /// Gets the order refund list asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;RefundResponse&gt;&gt;.</returns>
        Task<ListResponse<RefundResponse>> GetOrderRefundListAsync(string orderId, string from = null, int? limit = null);
    }
}
