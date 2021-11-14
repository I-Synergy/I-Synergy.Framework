using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Permission;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class PermissionsClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IPermissionsClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IPermissionsClient" />
    public class PermissionsClient : MollieClientBase, IPermissionsClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionsClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public PermissionsClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<PermissionsClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Gets the permission asynchronous.
        /// </summary>
        /// <param name="permissionId">The permission identifier.</param>
        /// <returns>Task&lt;PermissionResponse&gt;.</returns>
        public Task<PermissionResponse> GetPermissionAsync(string permissionId) =>
            _clientService.GetAsync<PermissionResponse>($"permissions/{permissionId}");

        /// <summary>
        /// Gets the permission asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;PermissionResponse&gt;.</returns>
        public Task<PermissionResponse> GetPermissionAsync(UrlObjectLink<PermissionResponse> url) =>
            _clientService.GetAsync(url);

        /// <summary>
        /// Gets the permission list asynchronous.
        /// </summary>
        /// <returns>Task&lt;ListResponse&lt;PermissionResponse&gt;&gt;.</returns>
        public Task<ListResponse<PermissionResponse>> GetPermissionListAsync() =>
            _clientService.GetListAsync<ListResponse<PermissionResponse>>("permissions", null, null);
    }
}
