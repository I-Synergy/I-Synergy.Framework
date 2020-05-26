using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Models.Chargeback;
using ISynergy.Framework.Payment.Mollie.Models.List;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IChargebacksClient
    /// </summary>
    public interface IChargebacksClient
    {
        /// <summary>
        /// Gets the chargeback asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="chargebackId">The chargeback identifier.</param>
        /// <returns>Task&lt;ChargebackResponse&gt;.</returns>
        Task<ChargebackResponse> GetChargebackAsync(string paymentId, string chargebackId);
        /// <summary>
        /// Gets the chargebacks list asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;ChargebackResponse&gt;&gt;.</returns>
        Task<ListResponse<ChargebackResponse>> GetChargebacksListAsync(string paymentId, string from = null, int? limit = null);
        /// <summary>
        /// Gets the chargebacks list asynchronous.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="testmode">if set to <c>true</c> [testmode].</param>
        /// <returns>Task&lt;ListResponse&lt;ChargebackResponse&gt;&gt;.</returns>
        Task<ListResponse<ChargebackResponse>> GetChargebacksListAsync(string profileId = null, bool? testmode = null);
    }
}
