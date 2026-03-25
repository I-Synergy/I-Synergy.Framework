using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models.Customer;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.Customer
{
    /// <summary>
    /// Class CustomerStorageClient.
    /// Implements the <see cref="ICustomerStorageClient" />
    /// </summary>
    /// <seealso cref="ICustomerStorageClient" />
    public class CustomerStorageClient : ICustomerStorageClient {
        /// <summary>
        /// The customer client
        /// </summary>
        private readonly ICustomerClient _customerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerStorageClient"/> class.
        /// </summary>
        /// <param name="customerClient">The customer client.</param>
        public CustomerStorageClient(ICustomerClient customerClient) {
            _customerClient = customerClient;
        }

        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public async Task Create(CreateCustomerModel model) {
            var customerRequest = new CustomerRequest {
                Name = model.Name,
                Email = model.Email
            };
            await _customerClient.CreateCustomerAsync(customerRequest);
        }
    }
}
