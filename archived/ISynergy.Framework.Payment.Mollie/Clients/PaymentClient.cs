﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISynergy.Framework.Payment.Extensions;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Request;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class PaymentClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IPaymentClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IPaymentClient" />
    public class PaymentClient : MollieClientBase, IPaymentClient
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public PaymentClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<PaymentClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Creates the payment asynchronous.
        /// </summary>
        /// <param name="paymentRequest">The payment request.</param>
        /// <returns>Task&lt;PaymentResponse&gt;.</returns>
        public Task<PaymentResponse> CreatePaymentAsync(PaymentRequest paymentRequest)
        {
            if (!string.IsNullOrWhiteSpace(paymentRequest.ProfileId) || paymentRequest.Testmode.HasValue || paymentRequest.ApplicationFee is not null)
            {
                ValidateApiKeyIsOauthAccesstoken();
            }

            return _clientService.PostAsync<PaymentResponse>("payments", paymentRequest);
        }

        /// <summary>
        /// Retrieve a single payment object by its payment identifier.
        /// </summary>
        /// <param name="paymentId">The payment's ID, for example tr_7UhSN1zuXS.</param>
        /// <param name="testmode">Oauth - Optional – Set this to true to get a payment made in test mode. If you omit this parameter, you can only retrieve live mode payments.</param>
        /// <returns>Task&lt;PaymentResponse&gt;.</returns>
        public Task<PaymentResponse> GetPaymentAsync(string paymentId, bool testmode = false)
        {
            if (testmode)
            {
                ValidateApiKeyIsOauthAccesstoken();
            }

            var testmodeParameter = testmode ? "?testmode=true" : string.Empty;

            return _clientService.GetAsync<PaymentResponse>($"payments/{paymentId}{testmodeParameter}");
        }

        /// <summary>
        /// Some payment methods are cancellable for an amount of time, usually until the next day. Or as long as the payment status is open. Payments may be cancelled manually from the Dashboard, or automatically by using this endpoint.
        /// </summary>
        /// <param name="paymentId">The payment identifier.</param>
        /// <returns>Task.</returns>
        public Task DeletePaymentAsync(string paymentId) =>
            _clientService.DeleteAsync($"payments/{paymentId}");

        /// <summary>
        /// Gets the payment asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;PaymentResponse&gt;.</returns>
        public Task<PaymentResponse> GetPaymentAsync(UrlObjectLink<PaymentResponse> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Gets the payment list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
        public Task<ListResponse<PaymentResponse>> GetPaymentListAsync(UrlObjectLink<ListResponse<PaymentResponse>> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Gets the payment list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="testMode">if set to <c>true</c> [test mode].</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
        public Task<ListResponse<PaymentResponse>> GetPaymentListAsync(string from = null, int? limit = null, string profileId = null, bool? testMode = null)
        {
            if (!string.IsNullOrWhiteSpace(profileId) || testMode.HasValue)
            {
                ValidateApiKeyIsOauthAccesstoken();
            }

            var parameters = new Dictionary<string, string>();
            parameters.AddValueIfNotNullOrEmpty(nameof(profileId), profileId);
            parameters.AddValueIfNotNullOrEmpty(nameof(testMode), Convert.ToString(testMode).ToLower());

            return _clientService.GetListAsync<ListResponse<PaymentResponse>>($"payments", from, limit, parameters);
        }
    }
}
