using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.Payment
{
    /// <summary>
    /// Class PaymentOverviewClient.
    /// Implements the <see cref="OverviewClientBase{PaymentResponse}" />
    /// Implements the <see cref="IPaymentOverviewClient" />
    /// </summary>
    /// <seealso cref="OverviewClientBase{PaymentResponse}" />
    /// <seealso cref="IPaymentOverviewClient" />
    public class PaymentOverviewClient : OverviewClientBase<PaymentResponse>, IPaymentOverviewClient {
        /// <summary>
        /// The payment client
        /// </summary>
        private readonly IPaymentClient _paymentClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentOverviewClient"/> class.
        /// </summary>
        /// <param name="paymentClient">The payment client.</param>
        public PaymentOverviewClient(IPaymentClient paymentClient) {
            _paymentClient = paymentClient;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns>Task&lt;OverviewModel&lt;PaymentResponse&gt;&gt;.</returns>
        public async Task<OverviewModel<PaymentResponse>> GetList() {
            return Map(await _paymentClient.GetPaymentListAsync());
        }

        /// <summary>
        /// Gets the list by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>OverviewModel&lt;PaymentResponse&gt;.</returns>
        public async Task<OverviewModel<PaymentResponse>> GetListByUrl(string url) {
            return Map(await _paymentClient.GetPaymentListAsync(CreateUrlObject(url)));
        }
    }
}
