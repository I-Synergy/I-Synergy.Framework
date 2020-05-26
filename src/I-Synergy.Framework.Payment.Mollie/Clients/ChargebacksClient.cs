using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISynergy.Framework.Payment.Extensions;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.Chargeback;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class ChargebacksClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IChargebacksClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IChargebacksClient" />
    public class ChargebacksClient : MollieClientBase, IChargebacksClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChargebacksClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public ChargebacksClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<ChargebacksClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Gets the chargeback asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="chargebackId">The chargeback identifier.</param>
        /// <returns>Task&lt;ChargebackResponse&gt;.</returns>
        public Task<ChargebackResponse> GetChargebackAsync(string paymentId, string chargebackId) =>
            _clientService.GetAsync<ChargebackResponse>($"payments/{paymentId}/chargebacks/{chargebackId}");

        /// <summary>
        /// Gets the chargebacks list asynchronous.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;ChargebackResponse&gt;&gt;.</returns>
        public Task<ListResponse<ChargebackResponse>> GetChargebacksListAsync(string paymentId, string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<ChargebackResponse>>($"payments/{paymentId}/chargebacks", from, limit);

        /// <summary>
        /// Gets the chargebacks list asynchronous.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="testmode">if set to <c>true</c> [testmode].</param>
        /// <returns>Task&lt;ListResponse&lt;ChargebackResponse&gt;&gt;.</returns>
        public Task<ListResponse<ChargebackResponse>> GetChargebacksListAsync(string profileId = null, bool? testmode = null)
        {
            if (profileId != null || testmode != null)
            {
                ValidateApiKeyIsOauthAccesstoken();
            }

            // Build parameters
            var parameters = new Dictionary<string, string>();
            parameters.AddValueIfNotNullOrEmpty(nameof(profileId), profileId);
            parameters.AddValueIfNotNullOrEmpty(nameof(testmode), Convert.ToString(testmode).ToLower());

            return _clientService.GetListAsync<ListResponse<ChargebackResponse>>($"chargebacks", null, null, parameters);
        }
    }
}
