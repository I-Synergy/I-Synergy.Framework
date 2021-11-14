using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Organization;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IOrganizationsClient
    /// </summary>
    public interface IOrganizationsClient
    {
        /// <summary>
        /// Gets the current organization asynchronous.
        /// </summary>
        /// <returns>Task&lt;OrganizationResponse&gt;.</returns>
        Task<OrganizationResponse> GetCurrentOrganizationAsync();
        /// <summary>
        /// Gets the organization asynchronous.
        /// </summary>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns>Task&lt;OrganizationResponse&gt;.</returns>
        Task<OrganizationResponse> GetOrganizationAsync(string organizationId);
        /// <summary>
        /// Gets the organizations list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;OrganizationResponse&gt;&gt;.</returns>
        Task<ListResponse<OrganizationResponse>> GetOrganizationsListAsync(string from = null, int? limit = null);
        /// <summary>
        /// Gets the organization asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;OrganizationResponse&gt;.</returns>
        Task<OrganizationResponse> GetOrganizationAsync(UrlObjectLink<OrganizationResponse> url);
    }
}
