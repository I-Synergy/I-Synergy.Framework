using ISynergy.Framework.Payment.Mollie.Models.Customer;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface ICustomerClient
    /// </summary>
    public interface ICustomerClient
    {
        /// <summary>
        /// Creates the customer asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;CustomerResponse&gt;.</returns>
        Task<CustomerResponse> CreateCustomerAsync(CustomerRequest request);
        /// <summary>
        /// Updates the customer asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;CustomerResponse&gt;.</returns>
        Task<CustomerResponse> UpdateCustomerAsync(string customerId, CustomerRequest request);
        /// <summary>
        /// Deletes the customer asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>Task.</returns>
        Task DeleteCustomerAsync(string customerId);
        /// <summary>
        /// Gets the customer asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>Task&lt;CustomerResponse&gt;.</returns>
        Task<CustomerResponse> GetCustomerAsync(string customerId);
        /// <summary>
        /// Gets the customer asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;CustomerResponse&gt;.</returns>
        Task<CustomerResponse> GetCustomerAsync(UrlObjectLink<CustomerResponse> url);
        /// <summary>
        /// Gets the customer list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;CustomerResponse&gt;&gt;.</returns>
        Task<ListResponse<CustomerResponse>> GetCustomerListAsync(UrlObjectLink<ListResponse<CustomerResponse>> url);
        /// <summary>
        /// Gets the customer list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;CustomerResponse&gt;&gt;.</returns>
        Task<ListResponse<CustomerResponse>> GetCustomerListAsync(string from = null, int? limit = null);
        /// <summary>
        /// Gets the customer payment list asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;PaymentResponse&gt;&gt;.</returns>
        Task<ListResponse<PaymentResponse>> GetCustomerPaymentListAsync(string customerId, string from = null, int? limit = null);
    }
}
