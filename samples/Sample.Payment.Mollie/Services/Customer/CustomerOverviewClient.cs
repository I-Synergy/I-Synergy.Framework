using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models.Customer;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.Customer
{
    /// <summary>
    /// Class CustomerOverviewClient.
    /// Implements the <see cref="OverviewClientBase{CustomerResponse}" />
    /// Implements the <see cref="ICustomerOverviewClient" />
    /// </summary>
    /// <seealso cref="OverviewClientBase{CustomerResponse}" />
    /// <seealso cref="ICustomerOverviewClient" />
    public class CustomerOverviewClient : OverviewClientBase<CustomerResponse>, ICustomerOverviewClient {
        /// <summary>
        /// The customer client
        /// </summary>
        private readonly ICustomerClient _customerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerOverviewClient"/> class.
        /// </summary>
        /// <param name="customerClient">The customer client.</param>
        public CustomerOverviewClient(ICustomerClient customerClient) {
            _customerClient = customerClient;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns>OverviewModel&lt;CustomerResponse&gt;.</returns>
        public async Task<OverviewModel<CustomerResponse>> GetList() {
            return Map(await _customerClient.GetCustomerListAsync());
        }

        /// <summary>
        /// Gets the list by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>OverviewModel&lt;CustomerResponse&gt;.</returns>
        public async Task<OverviewModel<CustomerResponse>> GetListByUrl(string url) {
            return Map(await _customerClient.GetCustomerListAsync(CreateUrlObject(url)));
        }
    }
}
