using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Mandate;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class MandateClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IMandateClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IMandateClient" />
    public class MandateClient : MollieClientBase, IMandateClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandateClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public MandateClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<MandateClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Gets the mandate asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="mandateId">The mandate identifier.</param>
        /// <returns>Task&lt;MandateResponse&gt;.</returns>
        public Task<MandateResponse> GetMandateAsync(string customerId, string mandateId) =>
            _clientService.GetAsync<MandateResponse>($"customers/{customerId}/mandates/{mandateId}");

        /// <summary>
        /// Gets the mandate list asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;MandateResponse&gt;&gt;.</returns>
        public Task<ListResponse<MandateResponse>> GetMandateListAsync(string customerId, string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<MandateResponse>>($"customers/{customerId}/mandates", from, limit);

        /// <summary>
        /// Creates the mandate asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;MandateResponse&gt;.</returns>
        public Task<MandateResponse> CreateMandateAsync(string customerId, MandateRequest request) =>
            _clientService.PostAsync<MandateResponse>($"customers/{customerId}/mandates", request);

        /// <summary>
        /// Gets the mandate list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;MandateResponse&gt;&gt;.</returns>
        public Task<ListResponse<MandateResponse>> GetMandateListAsync(UrlObjectLink<ListResponse<MandateResponse>> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Gets the mandate asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;MandateResponse&gt;.</returns>
        public Task<MandateResponse> GetMandateAsync(UrlObjectLink<MandateResponse> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Revokes the mandate asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="mandateId">The mandate identifier.</param>
        /// <returns>Task.</returns>
        public Task RevokeMandateAsync(string customerId, string mandateId) =>
            _clientService.DeleteAsync($"customers/{customerId}/mandates/{mandateId}");
    }
}
