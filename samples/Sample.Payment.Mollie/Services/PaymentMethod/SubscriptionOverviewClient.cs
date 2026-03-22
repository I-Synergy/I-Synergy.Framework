using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models.PaymentMethod;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.PaymentMethod
{
    /// <summary>
    /// Class PaymentMethodOverviewClient.
    /// Implements the <see cref="OverviewClientBase{PaymentMethodResponse}" />
    /// Implements the <see cref="IPaymentMethodOverviewClient" />
    /// </summary>
    /// <seealso cref="OverviewClientBase{PaymentMethodResponse}" />
    /// <seealso cref="IPaymentMethodOverviewClient" />
    public class PaymentMethodOverviewClient : OverviewClientBase<PaymentMethodResponse>, IPaymentMethodOverviewClient {
        /// <summary>
        /// The payment method client
        /// </summary>
        private readonly IPaymentMethodClient _paymentMethodClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodOverviewClient"/> class.
        /// </summary>
        /// <param name="paymentMethodClient">The payment method client.</param>
        public PaymentMethodOverviewClient(IPaymentMethodClient paymentMethodClient) {
            _paymentMethodClient = paymentMethodClient;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns>OverviewModel&lt;PaymentMethodResponse&gt;.</returns>
        public async Task<OverviewModel<PaymentMethodResponse>> GetList() {
            return Map(await _paymentMethodClient.GetPaymentMethodListAsync());
        }
    }
}
