namespace ISynergy.Framework.Core.Abstractions.Services
{
    /// <summary>
    /// Interface ITenantService
    /// </summary>
    public interface ITenantService
    {
        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        Guid TenantId { get; }
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        string UserName { get; }

        /// <summary>
        /// Sets the tenant.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        void SetTenant(Guid tenantId);
        /// <summary>
        /// Sets the tenant.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="username">The username.</param>
        void SetTenant(Guid tenantId, string username);
    }
}
