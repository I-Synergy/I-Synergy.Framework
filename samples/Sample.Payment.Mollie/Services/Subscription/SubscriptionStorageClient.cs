using System.Globalization;
using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models;
using ISynergy.Framework.Payment.Mollie.Models.Subscription;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.Subscription
{
    /// <summary>
    /// Class SubscriptionStorageClient.
    /// Implements the <see cref="ISubscriptionStorageClient" />
    /// </summary>
    /// <seealso cref="ISubscriptionStorageClient" />
    public class SubscriptionStorageClient : ISubscriptionStorageClient {
        /// <summary>
        /// The subscription client
        /// </summary>
        private readonly ISubscriptionClient _subscriptionClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionStorageClient"/> class.
        /// </summary>
        /// <param name="subscriptionClient">The subscription client.</param>
        public SubscriptionStorageClient(ISubscriptionClient subscriptionClient) {
            _subscriptionClient = subscriptionClient;
        }

        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public async Task Create(CreateSubscriptionModel model) {
            var subscriptionRequest = new SubscriptionRequest {
                Amount = new Amount(model.Currency, model.Amount.ToString(CultureInfo.InvariantCulture)),
                Interval = $"{model.IntervalAmount} {model.IntervalPeriod.ToString().ToLower()}",
                Description = model.Description,
                Times = model.Times
            };
            await _subscriptionClient.CreateSubscriptionAsync(model.CustomerId, subscriptionRequest);
        }
    }
}
