using System;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.EntityFramework.Attributes;

namespace ISynergy.Framework.EntityFramework.Entities
{
    /// <summary>
    /// Base Entity model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    [TenantAware(nameof(TenantId))]
    public abstract class BaseTenantEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        public Guid TenantId { get; set; }
    }
}
