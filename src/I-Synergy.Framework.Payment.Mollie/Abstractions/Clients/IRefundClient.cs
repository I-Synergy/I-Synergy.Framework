using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Refund;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IRefundClient
    /// </summary>
    public interface IRefundClient
    {
        /// <summary>
        /// Cancels the refund asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="refundId">The refund identifier.</param>
        /// <returns>Task.</returns>
        Task CancelRefundAsync(string paymentId, string refundId);
        /// <summary>
        /// Creates the refund asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="refundRequest">The refund request.</param>
        /// <returns>Task&lt;RefundResponse&gt;.</returns>
        Task<RefundResponse> CreateRefundAsync(string paymentId, RefundRequest refundRequest);
        /// <summary>
        /// Gets the refund asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="refundId">The refund identifier.</param>
        /// <returns>Task&lt;RefundResponse&gt;.</returns>
        Task<RefundResponse> GetRefundAsync(string paymentId, string refundId);
        /// <summary>
        /// Gets the refund list asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;RefundResponse&gt;&gt;.</returns>
        Task<ListResponse<RefundResponse>> GetRefundListAsync(string paymentId, string from = null, int? limit = null);
        /// <summary>
        /// Gets the refund asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;RefundResponse&gt;.</returns>
        Task<RefundResponse> GetRefundAsync(UrlObjectLink<RefundResponse> url);
    }
}
