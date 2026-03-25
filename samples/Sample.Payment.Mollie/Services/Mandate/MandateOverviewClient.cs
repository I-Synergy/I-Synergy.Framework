using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models.Mandate;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.Mandate
{
    /// <summary>
    /// Class MandateOverviewClient.
    /// Implements the <see cref="OverviewClientBase{MandateResponse}" />
    /// Implements the <see cref="IMandateOverviewClient" />
    /// </summary>
    /// <seealso cref="OverviewClientBase{MandateResponse}" />
    /// <seealso cref="IMandateOverviewClient" />
    public class MandateOverviewClient : OverviewClientBase<MandateResponse>, IMandateOverviewClient {
        /// <summary>
        /// The mandate client
        /// </summary>
        private readonly IMandateClient _mandateClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandateOverviewClient"/> class.
        /// </summary>
        /// <param name="mandateClient">The mandate client.</param>
        public MandateOverviewClient(IMandateClient mandateClient) {
            _mandateClient = mandateClient;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>Task&lt;OverviewModel&lt;MandateResponse&gt;&gt;.</returns>
        public async Task<OverviewModel<MandateResponse>> GetList(string customerId) {
            return Map(await _mandateClient.GetMandateListAsync(customerId));
        }

        /// <summary>
        /// Gets the list by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>OverviewModel&lt;MandateResponse&gt;.</returns>
        public async Task<OverviewModel<MandateResponse>> GetListByUrl(string url) {
            return Map(await _mandateClient.GetMandateListAsync(CreateUrlObject(url)));
        }
    }
}
