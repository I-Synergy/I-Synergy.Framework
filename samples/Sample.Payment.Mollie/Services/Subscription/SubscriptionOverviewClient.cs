using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models.Subscription;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.Subscription
{
    /// <summary>
    /// Class SubscriptionOverviewClient.
    /// Implements the <see cref="OverviewClientBase{SubscriptionResponse}" />
    /// Implements the <see cref="ISubscriptionOverviewClient" />
    /// </summary>
    /// <seealso cref="OverviewClientBase{SubscriptionResponse}" />
    /// <seealso cref="ISubscriptionOverviewClient" />
    public class SubscriptionOverviewClient : OverviewClientBase<SubscriptionResponse>, ISubscriptionOverviewClient {
        /// <summary>
        /// The subscription client
        /// </summary>
        private readonly ISubscriptionClient _subscriptionClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionOverviewClient"/> class.
        /// </summary>
        /// <param name="subscriptionClient">The subscription client.</param>
        public SubscriptionOverviewClient(ISubscriptionClient subscriptionClient) {
            _subscriptionClient = subscriptionClient;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>Task&lt;OverviewModel&lt;SubscriptionResponse&gt;&gt;.</returns>
        public async Task<OverviewModel<SubscriptionResponse>> GetList(string customerId) {
            return Map(await _subscriptionClient.GetSubscriptionListAsync(customerId));
        }

        /// <summary>
        /// Gets the list by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>OverviewModel&lt;SubscriptionResponse&gt;.</returns>
        public async Task<OverviewModel<SubscriptionResponse>> GetListByUrl(string url) {
            return Map(await _subscriptionClient.GetSubscriptionListAsync(CreateUrlObject(url)));
        }
    }
}
