using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Permission;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IPermissionsClient
    /// </summary>
    public interface IPermissionsClient
    {
        /// <summary>
        /// Gets the permission asynchronous.
        /// </summary>
        /// <param name="permissionId">The permission identifier.</param>
        /// <returns>Task&lt;PermissionResponse&gt;.</returns>
        Task<PermissionResponse> GetPermissionAsync(string permissionId);
        /// <summary>
        /// Gets the permission asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;PermissionResponse&gt;.</returns>
        Task<PermissionResponse> GetPermissionAsync(UrlObjectLink<PermissionResponse> url);
        /// <summary>
        /// Gets the permission list asynchronous.
        /// </summary>
        /// <returns>Task&lt;ListResponse&lt;PermissionResponse&gt;&gt;.</returns>
        Task<ListResponse<PermissionResponse>> GetPermissionListAsync();
    }
}
