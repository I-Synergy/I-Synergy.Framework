using ISynergy.Framework.EntityFramework.Entities;
using ISynergy.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Entities.Base
{
    /// <summary>
    /// Class BaseInternetAddress.
    /// Implements the <see cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    public abstract class BaseInternetAddress : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the type of the address.
        /// </summary>
        /// <value>The type of the address.</value>
        [Required] public InternetAddressTypes AddressType { get; set; }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        [Required] [StringLength(255)] public string Address { get; set; }
    }
}
