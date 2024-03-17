using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.Chargeback;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Refund;
using ISynergy.Framework.Payment.Mollie.Models.Settlement;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class SettlementsClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="ISettlementsClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="ISettlementsClient" />
    public class SettlementsClient : MollieClientBase, ISettlementsClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettlementsClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public SettlementsClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<SettlementsClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Gets the settlement asynchronous.
        /// </summary>
        /// <param name="settlementId">The settlement identifier.</param>
        /// <returns>Task&lt;SettlementResponse&gt;.</returns>
        public Task<SettlementResponse> GetSettlementAsync(string settlementId) =>
            _clientService.GetAsync<SettlementResponse>($"settlements/{settlementId}");

        /// <summary>
        /// Gets the next settlement asynchronous.
        /// </summary>
        /// <returns>Task&lt;SettlementResponse&gt;.</returns>
        public Task<SettlementResponse> GetNextSettlementAsync() =>
            _clientService.GetAsync<SettlementResponse>($"settlements/next");

        /// <summary>
        /// Gets the open balance asynchronous.
        /// </summary>
        /// <returns>Task&lt;SettlementResponse&gt;.</returns>
        public Task<SettlementResponse> GetOpenBalanceAsync() =>
            _clientService.GetAsync<SettlementResponse>($"settlements/open");

        /// <summary>
        /// Gets the settlements list asynchronous.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>Task&lt;ListResponse&lt;SettlementResponse&gt;&gt;.</returns>
        public Task<ListResponse<SettlementResponse>> GetSettlementsListAsync(string reference = null, string offset = null, int? count = null)
        {
            var queryString = !string.IsNullOrWhiteSpace(reference) ? $"?reference={reference}" : string.Empty;
            return _clientService.GetListAsync<ListResponse<SettlementResponse>>($"settlements{queryString}", offset, count);
        }

        /// <summary>
        /// Gets the settlement payments list asynchronous.
        /// </summary>
        /// <param name="settlementId">The settlement identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
        public Task<ListResponse<PaymentResponse>> GetSettlementPaymentsListAsync(string settlementId, string offset = null, int? count = null) =>
            _clientService.GetListAsync<ListResponse<PaymentResponse>>($"settlements/{settlementId}/payments", offset, count);

        /// <summary>
        /// Gets the settlement refunds list asynchronous.
        /// </summary>
        /// <param name="settlementId">The settlement identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>Task&lt;ListResponse&lt;RefundResponse&gt;&gt;.</returns>
        public Task<ListResponse<RefundResponse>> GetSettlementRefundsListAsync(string settlementId, string offset = null, int? count = null) =>
            _clientService.GetListAsync<ListResponse<RefundResponse>>($"settlements/{settlementId}/refunds", offset, count);

        /// <summary>
        /// Gets the settlement chargebacks list asynchronous.
        /// </summary>
        /// <param name="settlementId">The settlement identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>Task&lt;ListResponse&lt;ChargebackResponse&gt;&gt;.</returns>
        public Task<ListResponse<ChargebackResponse>> GetSettlementChargebacksListAsync(string settlementId, string offset = null, int? count = null) =>
            _clientService.GetListAsync<ListResponse<ChargebackResponse>>($"settlements/{settlementId}/chargebacks", offset, count);

        /// <summary>
        /// Gets the settlement asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;SettlementResponse&gt;.</returns>
        public Task<SettlementResponse> GetSettlementAsync(UrlObjectLink<SettlementResponse> url) =>
            _clientService.GetAsync(url);
    }
}
