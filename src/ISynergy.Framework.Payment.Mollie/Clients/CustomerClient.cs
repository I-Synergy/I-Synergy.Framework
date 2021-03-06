﻿using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.Customer;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class CustomerClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="ICustomerClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="ICustomerClient" />
    public class CustomerClient : MollieClientBase, ICustomerClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public CustomerClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<CustomerClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Creates the customer asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;CustomerResponse&gt;.</returns>
        public Task<CustomerResponse> CreateCustomerAsync(CustomerRequest request) =>
            _clientService.PostAsync<CustomerResponse>($"customers", request);

        /// <summary>
        /// Updates the customer asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;CustomerResponse&gt;.</returns>
        public Task<CustomerResponse> UpdateCustomerAsync(string customerId, CustomerRequest request) =>
            _clientService.PostAsync<CustomerResponse>($"customers/{customerId}", request);

        /// <summary>
        /// Deletes the customer asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>Task.</returns>
        public Task DeleteCustomerAsync(string customerId) =>
            _clientService.DeleteAsync($"customers/{customerId}");

        /// <summary>
        /// Gets the customer asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>Task&lt;CustomerResponse&gt;.</returns>
        public Task<CustomerResponse> GetCustomerAsync(string customerId) =>
            _clientService.GetAsync<CustomerResponse>($"customers/{customerId}");

        /// <summary>
        /// Gets the customer asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;CustomerResponse&gt;.</returns>
        public Task<CustomerResponse> GetCustomerAsync(UrlObjectLink<CustomerResponse> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Gets the customer list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;CustomerResponse&gt;&gt;.</returns>
        public Task<ListResponse<CustomerResponse>> GetCustomerListAsync(UrlObjectLink<ListResponse<CustomerResponse>> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Gets the customer list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;CustomerResponse&gt;&gt;.</returns>
        public Task<ListResponse<CustomerResponse>> GetCustomerListAsync(string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<CustomerResponse>>("customers", from, limit);

        /// <summary>
        /// Gets the customer payment list asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
        public Task<ListResponse<PaymentResponse>> GetCustomerPaymentListAsync(string customerId, string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<PaymentResponse>>($"customers/{customerId}/payments", from, limit);
    }
}
