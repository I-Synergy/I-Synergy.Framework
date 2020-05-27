using ISynergy.Framework.EntityFramework.Entities;
using ISynergy.Framework.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Entities.Base
{
    /// <summary>
    /// Class BasePhone.
    /// Implements the <see cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.EntityFramework.Entities.BaseTenantEntity" />
    public abstract class BasePhone : BaseTenantEntity
    {
        /// <summary>
        /// Gets or sets the type of the phone.
        /// </summary>
        /// <value>The type of the phone.</value>
        [Required] public PhoneTypes PhoneType { get; set; }
        /// <summary>
        /// Gets or sets the country prefix.
        /// </summary>
        /// <value>The country prefix.</value>
        [StringLength(128)] public string CountryPrefix { get; set; }
        /// <summary>
        /// Gets or sets the area code.
        /// </summary>
        /// <value>The area code.</value>
        [Required] [StringLength(5)] public string AreaCode { get; set; }
        /// <summary>
        /// Gets or sets the subscriber number.
        /// </summary>
        /// <value>The subscriber number.</value>
        [Required] [StringLength(10)] public string SubscriberNumber { get; set; }
    }
}
