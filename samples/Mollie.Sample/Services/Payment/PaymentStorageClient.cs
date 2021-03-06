﻿using System.Threading.Tasks;
using AutoMapper;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Request;
using Microsoft.Extensions.Configuration;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.Payment
{
    /// <summary>
    /// Class PaymentStorageClient.
    /// Implements the <see cref="IPaymentStorageClient" />
    /// </summary>
    /// <seealso cref="IPaymentStorageClient" />
    /// <autogeneratedoc />
    public class PaymentStorageClient : IPaymentStorageClient {
        /// <summary>
        /// The payment client
        /// </summary>
        /// <autogeneratedoc />
        private readonly IPaymentClient _paymentClient;
        /// <summary>
        /// The mapper
        /// </summary>
        /// <autogeneratedoc />
        private readonly IMapper _mapper;
        /// <summary>
        /// The configuration
        /// </summary>
        /// <autogeneratedoc />
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentStorageClient"/> class.
        /// </summary>
        /// <param name="paymentClient">The payment client.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="configuration">The configuration.</param>
        /// <autogeneratedoc />
        public PaymentStorageClient(IPaymentClient paymentClient, IMapper mapper, IConfiguration configuration) {
            _paymentClient = paymentClient;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <autogeneratedoc />
        public async Task Create(CreatePaymentModel model) {
            var paymentRequest = _mapper.Map<PaymentRequest>(model);
            paymentRequest.RedirectUrl = _configuration["DefaultRedirectUrl"];

            await _paymentClient.CreatePaymentAsync(paymentRequest);
        }
    }
}
