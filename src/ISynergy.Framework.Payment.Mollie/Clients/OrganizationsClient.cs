using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Organization;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class OrganizationsClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IOrganizationsClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IOrganizationsClient" />
    public class OrganizationsClient : MollieClientBase, IOrganizationsClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationsClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public OrganizationsClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<OrganizationsClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Gets the current organization asynchronous.
        /// </summary>
        /// <returns>Task&lt;OrganizationResponse&gt;.</returns>
        public Task<OrganizationResponse> GetCurrentOrganizationAsync() =>
            _clientService.GetAsync<OrganizationResponse>($"organizations/me");

        /// <summary>
        /// Gets the organization asynchronous.
        /// </summary>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns>Task&lt;OrganizationResponse&gt;.</returns>
        public Task<OrganizationResponse> GetOrganizationAsync(string organizationId) =>
            _clientService.GetAsync<OrganizationResponse>($"organizations/{organizationId}");

        /// <summary>
        /// Gets the organizations list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;OrganizationResponse&gt;&gt;.</returns>
        public Task<ListResponse<OrganizationResponse>> GetOrganizationsListAsync(string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<OrganizationResponse>>("organizations", from, limit, null);

        /// <summary>
        /// Gets the organization asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;OrganizationResponse&gt;.</returns>
        public Task<OrganizationResponse> GetOrganizationAsync(UrlObjectLink<OrganizationResponse> url) =>
            _clientService.GetAsync(url);
    }
}
