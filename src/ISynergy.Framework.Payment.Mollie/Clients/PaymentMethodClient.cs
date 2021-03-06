﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISynergy.Framework.Payment.Extensions;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using ISynergy.Framework.Payment.Mollie.Models;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.PaymentMethod;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class PaymentMethodClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IPaymentMethodClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IPaymentMethodClient" />
    public class PaymentMethodClient : MollieClientBase, IPaymentMethodClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public PaymentMethodClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<PaymentMethodClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Gets the payment method asynchronous.
        /// </summary>
        /// <param name="paymentMethod">The payment method.</param>
        /// <param name="includeIssuers">if set to <c>true</c> [include issuers].</param>
        /// <param name="locale">The locale.</param>
        /// <param name="includePricing">if set to <c>true</c> [include pricing].</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="testmode">if set to <c>true</c> [testmode].</param>
        /// <returns>Task&lt;PaymentMethodResponse&gt;.</returns>
        public Task<PaymentMethodResponse> GetPaymentMethodAsync(PaymentMethods paymentMethod, bool? includeIssuers = null, string locale = null, bool? includePricing = null, string profileId = null, bool? testmode = null)
        {
            var parameters = new Dictionary<string, string>();

            parameters.AddValueIfNotNullOrEmpty("locale", locale);
            AddOauthParameters(parameters, profileId, testmode);
            BuildIncludeParameter(parameters, includeIssuers, includePricing);

            return _clientService.GetAsync<PaymentMethodResponse>($"methods/{paymentMethod.ToString().ToLower()}{parameters.ToQueryString()}");
        }

        /// <summary>
        /// Gets all payment method list asynchronous.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="includeIssuers">if set to <c>true</c> [include issuers].</param>
        /// <param name="includePricing">if set to <c>true</c> [include pricing].</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentMethodResponse&gt;&gt;.</returns>
        public Task<ListResponse<PaymentMethodResponse>> GetAllPaymentMethodListAsync(string locale = null, bool? includeIssuers = null, bool? includePricing = null)
        {
            var parameters = new Dictionary<string, string>();

            parameters.AddValueIfNotNullOrEmpty("locale", locale);
            BuildIncludeParameter(parameters, includeIssuers, includePricing);

            return _clientService.GetListAsync<ListResponse<PaymentMethodResponse>>("methods/all", null, null, parameters);
        }

        /// <summary>
        /// Gets the payment method list asynchronous.
        /// </summary>
        /// <param name="sequenceType">Type of the sequence.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="includeIssuers">if set to <c>true</c> [include issuers].</param>
        /// <param name="includePricing">if set to <c>true</c> [include pricing].</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="testmode">if set to <c>true</c> [testmode].</param>
        /// <param name="resource">The resource.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentMethodResponse&gt;&gt;.</returns>
        public Task<ListResponse<PaymentMethodResponse>> GetPaymentMethodListAsync(SequenceType? sequenceType = null, string locale = null, Amount amount = null, bool? includeIssuers = null, bool? includePricing = null, string profileId = null, bool? testmode = null, Resource? resource = null)
        {
            var parameters = new Dictionary<string, string>() {
                {"sequenceType", sequenceType.ToString().ToLower()},
                {"locale", locale},
                {"amount[value]", amount?.Value},
                {"amount[currency]", amount?.Currency},
                {"resource", resource.ToString().ToLower()}
            };

            AddOauthParameters(parameters, profileId, testmode);
            BuildIncludeParameter(parameters, includeIssuers, includePricing);

            return _clientService.GetListAsync<ListResponse<PaymentMethodResponse>>("methods", null, null, parameters);
        }

        /// <summary>
        /// Gets the payment method asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;PaymentMethodResponse&gt;.</returns>
        public Task<PaymentMethodResponse> GetPaymentMethodAsync(UrlObjectLink<PaymentMethodResponse> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Adds the oauth parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="testmode">if set to <c>true</c> [testmode].</param>
        private void AddOauthParameters(Dictionary<string, string> parameters, string profileId = null, bool? testmode = null)
        {
            if (!string.IsNullOrWhiteSpace(profileId) || testmode.HasValue)
            {
                ValidateApiKeyIsOauthAccesstoken();

                parameters.AddValueIfNotNullOrEmpty("profileId", profileId);
                if (testmode.HasValue)
                {
                    parameters.AddValueIfNotNullOrEmpty("testmode", testmode.Value.ToString().ToLower());
                }
            }
        }

        /// <summary>
        /// Builds the include parameter.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="includeIssuers">if set to <c>true</c> [include issuers].</param>
        /// <param name="includePricing">if set to <c>true</c> [include pricing].</param>
        private void BuildIncludeParameter(Dictionary<string, string> parameters, bool? includeIssuers = null, bool? includePricing = null)
        {
            var includeList = new List<string>();

            if (includeIssuers == true)
            {
                includeList.Add("issuers");
            }

            if (includePricing == true)
            {
                includeList.Add("pricing");
            }

            if (includeList.Any())
            {
                parameters.Add("include", string.Join(",", includeList));
            }
        }
    }
}
