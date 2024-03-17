using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Models.Chargeback;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Refund;
using ISynergy.Framework.Payment.Mollie.Models.Settlement;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface ISettlementsClient
    /// </summary>
    public interface ISettlementsClient
    {
        /// <summary>
        /// Gets the settlement asynchronous.
        /// </summary>
        /// <param name="settlementId">The settlement identifier.</param>
        /// <returns>Task&lt;SettlementResponse&gt;.</returns>
        Task<SettlementResponse> GetSettlementAsync(string settlementId);
        /// <summary>
        /// Gets the next settlement asynchronous.
        /// </summary>
        /// <returns>Task&lt;SettlementResponse&gt;.</returns>
        Task<SettlementResponse> GetNextSettlementAsync();
        /// <summary>
        /// Gets the open balance asynchronous.
        /// </summary>
        /// <returns>Task&lt;SettlementResponse&gt;.</returns>
        Task<SettlementResponse> GetOpenBalanceAsync();
        /// <summary>
        /// Gets the settlements list asynchronous.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;SettlementResponse&gt;&gt;.</returns>
        Task<ListResponse<SettlementResponse>> GetSettlementsListAsync(string reference = null, string from = null, int? limit = null);
        /// <summary>
        /// Gets the settlement payments list asynchronous.
        /// </summary>
        /// <param name="settlementId">The settlement identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
        Task<ListResponse<PaymentResponse>> GetSettlementPaymentsListAsync(string settlementId, string from = null, int? limit = null);
        /// <summary>
        /// Gets the settlement refunds list asynchronous.
        /// </summary>
        /// <param name="settlementId">The settlement identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;RefundResponse&gt;&gt;.</returns>
        Task<ListResponse<RefundResponse>> GetSettlementRefundsListAsync(string settlementId, string from = null, int? limit = null);
        /// <summary>
        /// Gets the settlement chargebacks list asynchronous.
        /// </summary>
        /// <param name="settlementId">The settlement identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;ChargebackResponse&gt;&gt;.</returns>
        Task<ListResponse<ChargebackResponse>> GetSettlementChargebacksListAsync(string settlementId, string from = null, int? limit = null);
        /// <summary>
        /// Gets the settlement asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;SettlementResponse&gt;.</returns>
        Task<SettlementResponse> GetSettlementAsync(UrlObjectLink<SettlementResponse> url);
    }
}
