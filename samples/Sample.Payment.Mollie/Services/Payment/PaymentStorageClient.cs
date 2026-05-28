using System.Globalization;
using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Request;
using Microsoft.Extensions.Options;
using Mollie.Sample.Models;
using Mollie.Sample.Options;

namespace Mollie.Sample.Services.Payment
{
    /// <summary>
    /// Class PaymentStorageClient.
    /// Implements the <see cref="IPaymentStorageClient" />
    /// </summary>
    /// <seealso cref="IPaymentStorageClient" />
    public class PaymentStorageClient : IPaymentStorageClient {
        /// <summary>
        /// The payment client
        /// </summary>
        private readonly IPaymentClient _paymentClient;
        /// <summary>
        /// The payment options
        /// </summary>
        private readonly PaymentOptions _paymentOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentStorageClient"/> class.
        /// </summary>
        /// <param name="paymentClient">The payment client.</param>
        /// <param name="paymentOptions">The payment options.</param>
        public PaymentStorageClient(IPaymentClient paymentClient, IOptions<PaymentOptions> paymentOptions) {
            _paymentClient = paymentClient;
            _paymentOptions = paymentOptions.Value;
        }

        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public async Task Create(CreatePaymentModel model) {
            var paymentRequest = new PaymentRequest {
                Amount = new Amount(model.Currency, model.Amount.ToString(CultureInfo.InvariantCulture)),
                Description = model.Description,
                RedirectUrl = _paymentOptions.DefaultRedirectUrl
            };

            await _paymentClient.CreatePaymentAsync(paymentRequest);
        }
    }
}
